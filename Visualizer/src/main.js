import { SvgCanvas } from './render/svg-canvas.js';
import { PageManager } from './pages/page-manager.js';
import { createOrthogonalAxesPage } from './pages/orthogonal-axes.js';

// Boot
const svgEl = document.getElementById('canvas');
const canvas = new SvgCanvas(svgEl, {
  width: 560,
  height: 500,
  originX: 280,
  originY: 310,
  scale: 36,
});

const manager = new PageManager(
  canvas,
  document.getElementById('dots'),
  document.getElementById('prev'),
  document.getElementById('next'),
);

// Register pages — add new pages here
manager.addPage(createOrthogonalAxesPage());
