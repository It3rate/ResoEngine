import { Style } from '../core/types.js';

/**
 * Renders a single directed segment on an SVG canvas.
 * A segment has: dashed imaginary part → origin circle → solid real part → arrow.
 * Orientation: 'horizontal' or 'vertical'.
 *
 * Returns a handle object with references to all visual groups for drag binding.
 */
export class SegmentRenderer {
  /**
   * @param {import('../render/svg-canvas.js').SvgCanvas} canvas
   * @param {object} opts
   * @param {'horizontal'|'vertical'} opts.orientation
   * @param {object} opts.colors - { solid, grid, label }
   * @param {number} opts.crossPosition - Fixed position on the cross-axis (math units)
   */
  constructor(canvas, { orientation, colors, crossPosition = 0 }) {
    this.canvas = canvas;
    this.orientation = orientation;
    this.colors = colors;
    this.crossPosition = crossPosition;

    // Root group for the whole segment
    this.group = canvas.createGroup();

    // Sub-groups for layering
    this.dashGroup = canvas.createGroup();    // dashed imaginary line
    this.solidGroup = canvas.createGroup();   // solid real line
    this.circleGroup = canvas.createGroup();  // origin circle
    this.arrowGroup = canvas.createGroup();   // arrow head
    this.labelGroup = canvas.createGroup();   // text labels
    this.hitDotGroup = canvas.createGroup({ class: 'drag-handle' });   // invisible hit area for dot end
    this.hitBarGroup = canvas.createGroup({ class: 'drag-handle' });   // invisible hit area for bar
    this.hitArrowGroup = canvas.createGroup({ class: 'drag-handle' }); // invisible hit area for arrow end

    this.group.appendChild(this.dashGroup);
    this.group.appendChild(this.solidGroup);
    this.group.appendChild(this.circleGroup);
    this.group.appendChild(this.arrowGroup);
    this.group.appendChild(this.labelGroup);
    this.group.appendChild(this.hitDotGroup);
    this.group.appendChild(this.hitBarGroup);
    this.group.appendChild(this.hitArrowGroup);
  }

  /**
   * Update the visual to match the segment values.
   * @param {number} imaginary - Imaginary/negative extent
   * @param {number} real - Real/positive extent
   * @param {string} label - Display label
   */
  update(imaginary, real, label) {
    const { canvas, orientation, colors, crossPosition: cp } = this;
    const isH = orientation === 'horizontal';

    // Clear sub-groups
    for (const g of [this.dashGroup, this.solidGroup, this.circleGroup,
                      this.arrowGroup, this.labelGroup, this.hitDotGroup,
                      this.hitBarGroup, this.hitArrowGroup]) {
      while (g.firstChild) g.removeChild(g.firstChild);
    }

    // Pixel positions
    const originPx = canvas.mathToPixel(isH ? 0 : cp, isH ? cp : 0);
    const imagPx = canvas.mathToPixel(isH ? imaginary : cp, isH ? cp : imaginary);
    const realPx = canvas.mathToPixel(isH ? real : cp, isH ? cp : real);

    const sw = Style.strokeWidth;

    // --- Dashed line (imaginary end → origin) ---
    this.dashGroup.appendChild(canvas.createLine(
      imagPx.x, imagPx.y, originPx.x, originPx.y,
      { stroke: colors.solid, 'stroke-width': sw, 'stroke-dasharray': Style.dashArray }
    ));

    // --- Solid line (origin → real end, stopping short for arrow) ---
    const arrowLen = Style.arrowSize + 4;
    let solidEndX = realPx.x, solidEndY = realPx.y;
    if (isH) solidEndX = realPx.x - (real >= 0 ? arrowLen : -arrowLen);
    else solidEndY = realPx.y + (real >= 0 ? arrowLen : -arrowLen);

    this.solidGroup.appendChild(canvas.createLine(
      originPx.x, originPx.y, solidEndX, solidEndY,
      { stroke: colors.solid, 'stroke-width': sw }
    ));

    // --- Arrow ---
    const s = Style.arrowSize;
    let arrowPts;
    if (isH) {
      const dir = real >= 0 ? 1 : -1;
      const tipX = realPx.x + dir * 4;
      arrowPts = [
        [tipX, realPx.y],
        [tipX - dir * s * 1.5, realPx.y - s],
        [tipX - dir * s * 1.5, realPx.y + s],
      ];
    } else {
      const dir = real >= 0 ? -1 : 1; // pixel Y is inverted
      const tipY = realPx.y + dir * 4;
      arrowPts = [
        [realPx.x, tipY],
        [realPx.x - s, tipY - dir * s * 1.5],
        [realPx.x + s, tipY - dir * s * 1.5],
      ];
    }
    this.arrowGroup.appendChild(canvas.createPolygon(arrowPts, { fill: colors.solid }));

    // --- Dot at imaginary end (small filled circle) ---
    this.circleGroup.appendChild(canvas.createCircle(
      imagPx.x, imagPx.y, Style.circleRadius * 0.7,
      { fill: colors.solid, stroke: 'none' }
    ));

    // --- Origin circle (hollow) ---
    this.circleGroup.appendChild(canvas.createCircle(
      originPx.x, originPx.y, Style.circleRadius,
      { fill: '#ffffff', stroke: colors.solid, 'stroke-width': sw }
    ));

    // --- Labels ---
    const labelStyle = {
      fill: colors.label,
      'font-family': Style.fontFamily,
      'font-weight': Style.fontWeight,
      'font-size': Style.fontSize,
      'text-anchor': 'middle',
    };

    // Imaginary value label (at the imaginary endpoint / dot)
    const imagLabel = this._formatValue(imaginary, true);
    if (isH) {
      this.labelGroup.appendChild(canvas.createText(
        imagPx.x, imagPx.y + 30, imagLabel, labelStyle
      ));
    } else {
      this.labelGroup.appendChild(canvas.createText(
        imagPx.x - 30, imagPx.y + 7, imagLabel, { ...labelStyle, 'text-anchor': 'end' }
      ));
    }

    // Real value label
    const realLabel = this._formatValue(real, false);
    if (isH) {
      this.labelGroup.appendChild(canvas.createText(
        realPx.x, realPx.y + 30, realLabel, labelStyle
      ));
    } else {
      this.labelGroup.appendChild(canvas.createText(
        realPx.x - 30, realPx.y + 7, realLabel, { ...labelStyle, 'text-anchor': 'end' }
      ));
    }

    // Axis label (e.g., "A" or "B")
    if (label) {
      if (isH) {
        this.labelGroup.appendChild(canvas.createText(
          realPx.x + 30, realPx.y + 7, label, labelStyle
        ));
      } else {
        this.labelGroup.appendChild(canvas.createText(
          realPx.x, realPx.y - 20, label, labelStyle
        ));
      }
    }

    // --- Hit areas (invisible, larger targets for dragging) ---
    const hitPad = 14;

    // Dot (imaginary end) hit area
    this.hitDotGroup.appendChild(canvas.createCircle(
      imagPx.x, imagPx.y, hitPad,
      { fill: 'transparent', stroke: 'transparent', 'stroke-width': hitPad }
    ));
    this.hitDotGroup.classList.add(isH ? 'drag-handle-ew' : 'drag-handle-ns');

    // Arrow (real end) hit area
    this.hitArrowGroup.appendChild(canvas.createCircle(
      realPx.x, realPx.y, hitPad,
      { fill: 'transparent', stroke: 'transparent', 'stroke-width': hitPad }
    ));
    this.hitArrowGroup.classList.add(isH ? 'drag-handle-ew' : 'drag-handle-ns');

    // Bar (middle) hit area — a fat invisible line along the segment
    this.hitBarGroup.appendChild(canvas.createLine(
      imagPx.x, imagPx.y, realPx.x, realPx.y,
      { stroke: 'transparent', 'stroke-width': hitPad * 2 }
    ));
    this.hitBarGroup.classList.add('drag-handle');
  }

  _formatValue(val, isImaginary) {
    const rounded = Math.round(val * 10) / 10;
    const sign = rounded >= 0 ? '+' : '';
    if (isImaginary) {
      return `${sign}${rounded}i`;
    }
    return `${sign}${rounded}`;
  }
}
