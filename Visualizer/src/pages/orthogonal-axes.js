import { DirectedSegment, SegmentColors } from '../core/types.js';
import { GridRenderer } from '../render/grid-renderer.js';
import { SegmentBinding } from '../adapt/segment-binding.js';

/**
 * Page 1: Two directed segments joined orthogonally.
 * Segment A (red, horizontal) and Segment B (blue, vertical).
 * Grid lines show the OR combination of their real regions.
 * Endpoints are draggable; grid updates live.
 */
export function createOrthogonalAxesPage() {
  let gridRenderer = null;
  let bindingA = null;
  let bindingB = null;

  const segA = new DirectedSegment(-3, 5, 'A');
  const segB = new DirectedSegment(-2, 5, 'B');

  function rebuild() {
    if (gridRenderer) {
      gridRenderer.update(segA, segB, SegmentColors.red, SegmentColors.blue);
    }
  }

  return {
    title: 'Orthogonal Axes',

    init(canvas) {
      // Grid layer (behind segments)
      gridRenderer = new GridRenderer(canvas);
      canvas.append(gridRenderer.group);

      // Segment A: red, horizontal
      bindingA = new SegmentBinding(canvas, segA, {
        orientation: 'horizontal',
        colors: SegmentColors.red,
        crossPosition: 0,
        onChange: rebuild,
      });
      canvas.append(bindingA.group);

      // Segment B: blue, vertical
      bindingB = new SegmentBinding(canvas, segB, {
        orientation: 'vertical',
        colors: SegmentColors.blue,
        crossPosition: 0,
        onChange: rebuild,
      });
      canvas.append(bindingB.group);

      // Initial grid render
      rebuild();
    },

    destroy() {
      if (bindingA) { bindingA.destroy(); bindingA = null; }
      if (bindingB) { bindingB.destroy(); bindingB = null; }
      gridRenderer = null;
    },
  };
}
