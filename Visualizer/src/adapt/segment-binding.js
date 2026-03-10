import { SegmentRenderer } from '../render/segment-renderer.js';
import { DragHandler } from '../input/drag-handler.js';

/**
 * Binds a DirectedSegment (data) to a SegmentRenderer (visual) + DragHandlers (input).
 * Three drag zones: dot (imaginary end), arrow (real end), bar (whole segment).
 * Snaps to 0.5 increments for clean values.
 */
export class SegmentBinding {
  /**
   * @param {import('../render/svg-canvas.js').SvgCanvas} canvas
   * @param {import('../core/types.js').DirectedSegment} segment
   * @param {object} opts
   * @param {'horizontal'|'vertical'} opts.orientation
   * @param {object} opts.colors
   * @param {number} [opts.crossPosition=0]
   * @param {(seg: DirectedSegment) => void} [opts.onChange]
   * @param {number} [opts.snapIncrement=0.5]
   */
  constructor(canvas, segment, { orientation, colors, crossPosition = 0, onChange, snapIncrement = 0.5 }) {
    this.canvas = canvas;
    this.segment = segment;
    this.orientation = orientation;
    this.onChange = onChange || (() => {});
    this.snapIncrement = snapIncrement;

    this.renderer = new SegmentRenderer(canvas, { orientation, colors, crossPosition });
    this.dragHandlers = [];

    // Initial render
    this.render();

    // Set up drag handlers after first render (so hit groups exist)
    this._setupDrag();
  }

  render() {
    this.renderer.update(this.segment.imaginary, this.segment.real, this.segment.label);
  }

  _snap(val) {
    const inc = this.snapIncrement;
    return Math.round(val / inc) * inc;
  }

  _setupDrag() {
    const isH = this.orientation === 'horizontal';
    const axis = isH ? 'horizontal' : 'vertical';

    // Drag the dot (imaginary end)
    const dotDrag = new DragHandler(this.canvas, this.renderer.hitDotGroup, {
      axis,
      onMove: ({ mx, my }) => {
        const delta = isH ? mx : my;
        this.segment.imaginary = this._snap(this.segment.imaginary + delta);
        this.render();
        this.onChange(this.segment);
      },
    });
    this.dragHandlers.push(dotDrag);

    // Drag the arrow (real end)
    const arrowDrag = new DragHandler(this.canvas, this.renderer.hitArrowGroup, {
      axis,
      onMove: ({ mx, my }) => {
        const delta = isH ? mx : my;
        this.segment.real = this._snap(this.segment.real + delta);
        this.render();
        this.onChange(this.segment);
      },
    });
    this.dragHandlers.push(arrowDrag);

    // Drag the bar (whole segment)
    const barDrag = new DragHandler(this.canvas, this.renderer.hitBarGroup, {
      axis,
      onMove: ({ mx, my }) => {
        const delta = isH ? mx : my;
        this.segment.imaginary = this._snap(this.segment.imaginary + delta);
        this.segment.real = this._snap(this.segment.real + delta);
        this.render();
        this.onChange(this.segment);
      },
    });
    this.dragHandlers.push(barDrag);
  }

  /** Get the root SVG group for this segment */
  get group() {
    return this.renderer.group;
  }

  destroy() {
    for (const h of this.dragHandlers) h.destroy();
    this.dragHandlers = [];
    if (this.renderer.group.parentNode) {
      this.renderer.group.parentNode.removeChild(this.renderer.group);
    }
  }
}
