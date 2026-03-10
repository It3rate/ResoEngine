const SVG_NS = 'http://www.w3.org/2000/svg';

/**
 * Manages an SVG element with a math↔pixel coordinate system.
 * Origin is placed at a configurable position; Y-axis is flipped (math up = pixel up).
 */
export class SvgCanvas {
  /**
   * @param {SVGSVGElement} svg - The SVG DOM element
   * @param {object} opts
   * @param {number} opts.width - ViewBox width
   * @param {number} opts.height - ViewBox height
   * @param {number} opts.originX - Pixel X of math origin (0,0)
   * @param {number} opts.originY - Pixel Y of math origin (0,0)
   * @param {number} opts.scale - Pixels per math unit
   */
  constructor(svg, { width = 560, height = 500, originX = 280, originY = 300, scale = 40 } = {}) {
    this.svg = svg;
    this.width = width;
    this.height = height;
    this.originX = originX;
    this.originY = originY;
    this.scale = scale;

    svg.setAttribute('viewBox', `0 0 ${width} ${height}`);
  }

  /** Convert math coordinates to pixel coordinates */
  mathToPixel(mx, my) {
    return {
      x: this.originX + mx * this.scale,
      y: this.originY - my * this.scale, // flip Y
    };
  }

  /** Convert pixel coordinates to math coordinates */
  pixelToMath(px, py) {
    return {
      x: (px - this.originX) / this.scale,
      y: (this.originY - py) / this.scale, // flip Y
    };
  }

  /** Convert a math distance to pixel distance */
  mathToPixelDist(d) { return d * this.scale; }

  /** Get mouse/touch position in SVG viewBox coordinates */
  pointerToSvg(event) {
    const pt = this.svg.createSVGPoint();
    const source = event.touches ? event.touches[0] : event;
    pt.x = source.clientX;
    pt.y = source.clientY;
    const ctm = this.svg.getScreenCTM().inverse();
    const svgPt = pt.matrixTransform(ctm);
    return { x: svgPt.x, y: svgPt.y };
  }

  // --- SVG element factories ---

  createElement(tag, attrs = {}) {
    const el = document.createElementNS(SVG_NS, tag);
    for (const [k, v] of Object.entries(attrs)) {
      el.setAttribute(k, String(v));
    }
    return el;
  }

  createGroup(attrs = {}) {
    return this.createElement('g', attrs);
  }

  createLine(x1, y1, x2, y2, attrs = {}) {
    return this.createElement('line', { x1, y1, x2, y2, ...attrs });
  }

  createRect(x, y, width, height, attrs = {}) {
    return this.createElement('rect', { x, y, width, height, ...attrs });
  }

  createCircle(cx, cy, r, attrs = {}) {
    return this.createElement('circle', { cx, cy, r, ...attrs });
  }

  createPolygon(points, attrs = {}) {
    const ptStr = points.map(([x, y]) => `${x},${y}`).join(' ');
    return this.createElement('polygon', { points: ptStr, ...attrs });
  }

  createText(x, y, text, attrs = {}) {
    const el = this.createElement('text', { x, y, ...attrs });
    el.textContent = text;
    return el;
  }

  /** Clear all children of the SVG */
  clear() {
    while (this.svg.firstChild) this.svg.removeChild(this.svg.firstChild);
  }

  /** Append element to SVG */
  append(el) {
    this.svg.appendChild(el);
    return el;
  }
}
