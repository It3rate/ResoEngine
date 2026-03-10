/**
 * Generic SVG drag handler supporting mouse and touch.
 * Constrains movement to a single axis if specified.
 */
export class DragHandler {
  /**
   * @param {import('../render/svg-canvas.js').SvgCanvas} canvas
   * @param {SVGElement} element - The SVG element to make draggable
   * @param {object} opts
   * @param {'horizontal'|'vertical'|'free'} [opts.axis='free']
   * @param {(delta: {dx: number, dy: number, mx: number, my: number}) => void} opts.onMove
   * @param {() => void} [opts.onStart]
   * @param {() => void} [opts.onEnd]
   */
  constructor(canvas, element, { axis = 'free', onMove, onStart, onEnd }) {
    this.canvas = canvas;
    this.element = element;
    this.axis = axis;
    this.onMove = onMove;
    this.onStart = onStart || (() => {});
    this.onEnd = onEnd || (() => {});

    this._startPx = null;
    this._dragging = false;

    // Bind handlers
    this._onPointerDown = this._onPointerDown.bind(this);
    this._onPointerMove = this._onPointerMove.bind(this);
    this._onPointerUp = this._onPointerUp.bind(this);

    element.addEventListener('mousedown', this._onPointerDown);
    element.addEventListener('touchstart', this._onPointerDown, { passive: false });
  }

  _onPointerDown(e) {
    e.preventDefault();
    e.stopPropagation();
    this._dragging = true;
    this._startPx = this.canvas.pointerToSvg(e);
    this.onStart();

    document.addEventListener('mousemove', this._onPointerMove);
    document.addEventListener('mouseup', this._onPointerUp);
    document.addEventListener('touchmove', this._onPointerMove, { passive: false });
    document.addEventListener('touchend', this._onPointerUp);
  }

  _onPointerMove(e) {
    if (!this._dragging) return;
    e.preventDefault();

    const current = this.canvas.pointerToSvg(e);
    let dx = current.x - this._startPx.x;
    let dy = current.y - this._startPx.y;

    // Constrain to axis
    if (this.axis === 'horizontal') dy = 0;
    if (this.axis === 'vertical') dx = 0;

    // Convert pixel delta to math delta
    const mx = dx / this.canvas.scale;
    const my = -dy / this.canvas.scale; // flip Y

    this._startPx = current;
    this.onMove({ dx, dy, mx, my });
  }

  _onPointerUp(e) {
    if (!this._dragging) return;
    this._dragging = false;
    this.onEnd();

    document.removeEventListener('mousemove', this._onPointerMove);
    document.removeEventListener('mouseup', this._onPointerUp);
    document.removeEventListener('touchmove', this._onPointerMove);
    document.removeEventListener('touchend', this._onPointerUp);
  }

  /** Remove all event listeners */
  destroy() {
    this.element.removeEventListener('mousedown', this._onPointerDown);
    this.element.removeEventListener('touchstart', this._onPointerDown);
    document.removeEventListener('mousemove', this._onPointerMove);
    document.removeEventListener('mouseup', this._onPointerUp);
    document.removeEventListener('touchmove', this._onPointerMove);
    document.removeEventListener('touchend', this._onPointerUp);
  }
}
