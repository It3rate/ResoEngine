/**
 * A directed segment: a 1D value with imaginary (start/dot) and real (end/arrow) extents.
 * Maps to Proportion/Axis in ResoEngine — imaginary is Con, real is Pro.
 */
export class DirectedSegment {
  /**
   * @param {number} imaginary - The negative/imaginary extent (e.g., -3)
   * @param {number} real - The positive/real extent (e.g., 5)
   * @param {string} label - Display name (e.g., "A")
   */
  constructor(imaginary, real, label = '') {
    this.imaginary = imaginary;
    this.real = real;
    this.label = label;
  }

  /** Total span from imaginary to real */
  get span() { return this.real - this.imaginary; }

  /** Clone this segment */
  clone() { return new DirectedSegment(this.imaginary, this.real, this.label); }
}

/** Color configuration for a segment and its grid */
export const SegmentColors = {
  red: {
    solid: '#C00000',
    grid: '#FFB3B3',
    label: '#C00000',
  },
  blue: {
    solid: '#0020C0',
    grid: '#A6C8FF',
    label: '#0020C0',
  },
};

/** Shared style constants matching the reference SVG */
export const Style = {
  strokeWidth: 4,
  gridStrokeWidth: 2,
  dashArray: '8,6',
  circleRadius: 7,
  arrowSize: 12,
  fontSize: 22,
  fontFamily: 'Arial, sans-serif',
  fontWeight: 'bold',
  fillColor: '#FFFACD',
  gridSpacing: 1, // one grid line per math unit
};
