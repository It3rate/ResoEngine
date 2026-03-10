import { Style } from '../core/types.js';

/**
 * Renders the orthogonal grid for two directed segments.
 * - Horizontal colored lines in the V-segment's real region
 * - Vertical colored lines in the H-segment's real region
 * - Yellow fill in the quadrant where both are imaginary
 */
export class GridRenderer {
  /**
   * @param {import('../render/svg-canvas.js').SvgCanvas} canvas
   */
  constructor(canvas) {
    this.canvas = canvas;
    this.group = canvas.createGroup();
  }

  /**
   * Update the grid based on current segment values.
   * @param {object} hSeg - Horizontal segment { imaginary, real }
   * @param {object} vSeg - Vertical segment { imaginary, real }
   * @param {object} hColors - Horizontal segment colors { grid }
   * @param {object} vColors - Vertical segment colors { grid }
   */
  update(hSeg, vSeg, hColors, vColors) {
    const { canvas } = this;
    const g = this.group;

    // Clear
    while (g.firstChild) g.removeChild(g.firstChild);

    const sw = Style.gridStrokeWidth;
    const spacing = Style.gridSpacing;

    // Full extents in math space
    const xMin = hSeg.imaginary;
    const xMax = hSeg.real;
    const yMin = vSeg.imaginary;
    const yMax = vSeg.real;

    // Pixel bounds
    const topLeft = canvas.mathToPixel(xMin, yMax);
    const botRight = canvas.mathToPixel(xMax, yMin);

    // --- Yellow fill: quadrant where both are imaginary (x < 0 AND y < 0) ---
    const origin = canvas.mathToPixel(0, 0);
    const imagCorner = canvas.mathToPixel(xMin, yMin);

    if (xMin < 0 && yMin < 0) {
      const fillX = imagCorner.x;
      const fillY = origin.y; // y=0 in pixels (top of the imaginary-Y region since Y is flipped from the math-positive above)
      const fillW = origin.x - imagCorner.x;
      const fillH = imagCorner.y - origin.y;
      if (fillW > 0 && fillH > 0) {
        g.appendChild(canvas.createRect(fillX, fillY, fillW, fillH, {
          fill: Style.fillColor, stroke: 'none'
        }));
      }
    }

    // --- Horizontal grid lines (V-segment's color, in V's real region) ---
    // These are horizontal lines spanning the full X range of H-segment
    // Drawn for each integer Y in V's real region (y > 0 up to yMax)
    {
      const startY = Math.max(Math.ceil(0), Math.ceil(yMin));
      const endY = Math.floor(yMax);
      for (let my = startY; my <= endY; my++) {
        if (my === 0) continue; // skip origin line
        const left = canvas.mathToPixel(xMin, my);
        const right = canvas.mathToPixel(xMax, my);
        g.appendChild(canvas.createLine(
          left.x, left.y, right.x, right.y,
          { stroke: vColors.grid, 'stroke-width': sw }
        ));
      }
    }

    // --- Vertical grid lines (H-segment's color, in H's real region) ---
    // These are vertical lines spanning the full Y range of V-segment
    // Drawn for each integer X in H's real region (x > 0 up to xMax)
    {
      const startX = Math.max(Math.ceil(0), Math.ceil(xMin));
      const endX = Math.floor(xMax);
      for (let mx = startX; mx <= endX; mx++) {
        if (mx === 0) continue; // skip origin line
        const top = canvas.mathToPixel(mx, yMax);
        const bot = canvas.mathToPixel(mx, yMin);
        g.appendChild(canvas.createLine(
          top.x, top.y, bot.x, bot.y,
          { stroke: hColors.grid, 'stroke-width': sw }
        ));
      }
    }
  }
}
