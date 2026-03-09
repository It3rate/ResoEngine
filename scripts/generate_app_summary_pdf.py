from __future__ import annotations

from pathlib import Path


PAGE_WIDTH = 612
PAGE_HEIGHT = 792
MARGIN = 42


def escape_pdf_text(value: str) -> str:
    return value.replace('\\', '\\\\').replace('(', '\\(').replace(')', '\\)')


def estimate_width(text: str, font_size: int) -> float:
    return len(text) * font_size * 0.5


def wrap_text(text: str, font_size: int, max_width: float) -> list[str]:
    words = text.split()
    if not words:
        return ['']

    lines: list[str] = []
    current = words[0]
    for word in words[1:]:
        candidate = f'{current} {word}'
        if estimate_width(candidate, font_size) <= max_width:
            current = candidate
        else:
            lines.append(current)
            current = word
    lines.append(current)
    return lines


class PdfPage:
    def __init__(self) -> None:
        self.parts: list[str] = []

    def text(self, x: float, y: float, text: str, font: str, size: int) -> None:
        self.parts.append(
            f'BT /{font} {size} Tf 1 0 0 1 {x:.2f} {y:.2f} Tm ({escape_pdf_text(text)}) Tj ET'
        )

    def line(self, x1: float, y1: float, x2: float, y2: float, width: float = 1) -> None:
        self.parts.append(f'{width:.2f} w {x1:.2f} {y1:.2f} m {x2:.2f} {y2:.2f} l S')

    def rect_fill(self, x: float, y: float, width: float, height: float, gray: float) -> None:
        self.parts.append(f'{gray:.2f} g {x:.2f} {y:.2f} {width:.2f} {height:.2f} re f 0 g')

    def render(self) -> bytes:
        return '\n'.join(self.parts).encode('ascii', 'replace')


def build_pdf(output_path: Path) -> None:
    output_path.parent.mkdir(parents=True, exist_ok=True)

    page = PdfPage()
    y = PAGE_HEIGHT - MARGIN

    page.text(MARGIN, y, 'ResoEngine Repo Summary', 'F2', 18)
    y -= 12
    page.line(MARGIN, y, PAGE_WIDTH - MARGIN, y, 1)
    y -= 20

    sections = [
        (
            'What it is',
            [
                'A small .NET 10 class library for modeling proportions, axes, chirality, and algebra-style composition in the ResoEngine namespace.',
                'Repo evidence points to an in-progress core domain engine rather than a complete end-user application.',
            ],
        ),
        (
            'Who it is for',
            [
                'Primary persona: Not found in repo.',
                'Inferred audience: .NET developers or researchers working on mathematical or spatial value modeling.',
            ],
        ),
        (
            'What it does',
            [
                'Represents proportional values with numerator, denominator, chirality, and end-lock state.',
                'Creates rational, imaginary, fixed rational, and fixed imaginary proportions.',
                'Flips chirality and reads ticks from different perspectives.',
                'Builds a 4-component Axis from paired left and right proportions.',
                'Applies axis multiplication rules through AlgebraEntry mappings and signed accumulation.',
                'Exposes common interfaces for values, proportions, spaces, and subspaces.',
                'Includes extension points for child values and frame elements, but several methods are not yet implemented.',
            ],
        ),
        (
            'How it works',
            [
                'Single project: Core/Core.csproj targets net10.0 and produces Core.dll.',
                'Domain layer: Proportion, Axis, Measure, and AlgebraTable live in the ResoEngine namespace.',
                'Support layer: Chirality, EndLock, PVRef, AlgebraEntry, IValue, IProportion, ISpace, and ISubSpace define behavior contracts.',
                'Data flow: callers create Proportion values, Axis normalizes them to Con and Pro sides, then algebra entries map component multiplication into a result vector.',
                'Services, UI, persistence, API host, tests, and external data sources: Not found in repo.',
            ],
        ),
        (
            'How to run',
            [
                '1. Open C:/Users/Robin/source/repos/ResoEngine/ResoEngine.slnx or build C:/Users/Robin/source/repos/ResoEngine/Core/Core.csproj.',
                '2. Verified build command in this environment: dotnet build C:/Users/Robin/source/repos/ResoEngine/Core/Core.csproj --no-restore',
                '3. Runnable entry point or sample usage app: Not found in repo.',
            ],
        ),
    ]

    heading_size = 12
    body_size = 9
    heading_gap = 14
    body_gap = 11
    bullet_indent = 10
    text_width = PAGE_WIDTH - (2 * MARGIN) - 14

    for title, bullets in sections:
        page.rect_fill(MARGIN - 4, y - 4, PAGE_WIDTH - 2 * (MARGIN - 4), 16, 0.93)
        page.text(MARGIN, y, title, 'F2', heading_size)
        y -= heading_gap
        for bullet in bullets:
            wrapped = wrap_text(bullet, body_size, text_width - bullet_indent)
            page.text(MARGIN, y, '-', 'F1', body_size)
            page.text(MARGIN + bullet_indent, y, wrapped[0], 'F1', body_size)
            y -= body_gap
            for continuation in wrapped[1:]:
                page.text(MARGIN + bullet_indent, y, continuation, 'F1', body_size)
                y -= body_gap
        y -= 6

    if y < MARGIN:
        raise RuntimeError('Content overflowed the single page layout.')

    content = page.render()
    objects: list[bytes] = []
    objects.append(b'<< /Type /Catalog /Pages 2 0 R >>')
    objects.append(b'<< /Type /Pages /Kids [3 0 R] /Count 1 >>')
    objects.append(
        f'<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {PAGE_WIDTH} {PAGE_HEIGHT}] /Resources << /Font << /F1 4 0 R /F2 5 0 R >> >> /Contents 6 0 R >>'.encode(
            'ascii'
        )
    )
    objects.append(b'<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>')
    objects.append(b'<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>')
    objects.append(f'<< /Length {len(content)} >>\nstream\n'.encode('ascii') + content + b'\nendstream')

    pdf = bytearray()
    pdf.extend(b'%PDF-1.4\n')
    offsets = [0]
    for index, obj in enumerate(objects, start=1):
        offsets.append(len(pdf))
        pdf.extend(f'{index} 0 obj\n'.encode('ascii'))
        pdf.extend(obj)
        pdf.extend(b'\nendobj\n')

    xref_start = len(pdf)
    pdf.extend(f'xref\n0 {len(objects) + 1}\n'.encode('ascii'))
    pdf.extend(b'0000000000 65535 f \n')
    for offset in offsets[1:]:
        pdf.extend(f'{offset:010d} 00000 n \n'.encode('ascii'))

    pdf.extend(
        f'trailer\n<< /Size {len(objects) + 1} /Root 1 0 R >>\nstartxref\n{xref_start}\n%%EOF\n'.encode(
            'ascii'
        )
    )

    output_path.write_bytes(pdf)


if __name__ == '__main__':
    build_pdf(Path('output/pdf/resoengine-summary.pdf'))
