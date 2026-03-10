/**
 * Manages page navigation with next/prev buttons and dot indicators.
 * Pages are objects with: { title: string, init(canvas): void, destroy(): void }
 */
export class PageManager {
  /**
   * @param {import('../render/svg-canvas.js').SvgCanvas} canvas
   * @param {HTMLElement} dotsContainer
   * @param {HTMLButtonElement} prevBtn
   * @param {HTMLButtonElement} nextBtn
   */
  constructor(canvas, dotsContainer, prevBtn, nextBtn) {
    this.canvas = canvas;
    this.dotsContainer = dotsContainer;
    this.prevBtn = prevBtn;
    this.nextBtn = nextBtn;

    /** @type {Array<{title: string, init: (canvas: SvgCanvas) => void, destroy: () => void}>} */
    this.pages = [];
    this.currentIndex = -1;

    prevBtn.addEventListener('click', () => this.prev());
    nextBtn.addEventListener('click', () => this.next());
  }

  /**
   * Add a page to the manager.
   * @param {object} page - { title, init(canvas), destroy() }
   */
  addPage(page) {
    this.pages.push(page);
    this._renderDots();
    // If this is the first page, show it
    if (this.pages.length === 1) this.goTo(0);
  }

  goTo(index) {
    if (index < 0 || index >= this.pages.length) return;

    // Destroy current page
    if (this.currentIndex >= 0 && this.pages[this.currentIndex]) {
      this.pages[this.currentIndex].destroy();
    }

    this.canvas.clear();
    this.currentIndex = index;

    // Init new page
    this.pages[index].init(this.canvas);

    this._renderDots();
    this._updateButtons();
  }

  next() { this.goTo(this.currentIndex + 1); }
  prev() { this.goTo(this.currentIndex - 1); }

  _renderDots() {
    const container = this.dotsContainer;
    container.innerHTML = '';
    this.pages.forEach((_, i) => {
      const dot = document.createElement('span');
      dot.className = 'dot' + (i === this.currentIndex ? ' active' : '');
      dot.addEventListener('click', () => this.goTo(i));
      container.appendChild(dot);
    });
  }

  _updateButtons() {
    this.prevBtn.disabled = this.currentIndex <= 0;
    this.nextBtn.disabled = this.currentIndex >= this.pages.length - 1;
  }
}
