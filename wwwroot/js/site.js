// Código do carrossel – carregado no <head> do layout, usa DOMContentLoaded para aguardar a árvore DOM.
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('.carrossel-filmes').forEach(carrossel => {
    const linha = carrossel.querySelector('.filme-linha');
    if (!linha) return;

    const esquerda = carrossel.querySelector('.seta-esquerda');
    const direita = carrossel.querySelector('.seta-direita');

    // calcula passo de rolagem com base no tamanho do primeiro card (fallback 300)
    const primeiroCard = linha.querySelector('.filme-card');
    let cardWidth = 300;
    if (primeiroCard) {
      const rect = primeiroCard.getBoundingClientRect();
      const style = getComputedStyle(primeiroCard);
      const marginRight = parseFloat(style.marginRight || '0');
      cardWidth = Math.round(rect.width + marginRight);
    }
    const scrollStep = cardWidth;

    // handlers das setas
    const toLeft = () => linha.scrollBy({ left: -scrollStep, behavior: 'smooth' });
    const toRight = () => linha.scrollBy({ left: scrollStep, behavior: 'smooth' });

    if (esquerda) esquerda.addEventListener('click', toLeft);
    if (direita) direita.addEventListener('click', toRight);

    // acessibilidade por teclado
    carrossel.tabIndex = 0;
    carrossel.addEventListener('keydown', (e) => {
      if (e.key === 'ArrowLeft') { e.preventDefault(); toLeft(); }
      if (e.key === 'ArrowRight') { e.preventDefault(); toRight(); }
    });

    // rolagem automática (um card a cada intervalo) com pausa em hover/focus
    const intervalMs = 4000;
    let auto = null;
    const startAuto = () => {
      if (auto) return;
      auto = setInterval(() => {
        if (linha.scrollWidth <= linha.clientWidth) return;
        const maxLeft = linha.scrollWidth - linha.clientWidth;
        const nextLeft = Math.min(linha.scrollLeft + scrollStep, maxLeft);
        linha.scrollTo({ left: nextLeft, behavior: 'smooth' });
        if (nextLeft >= maxLeft - 1) {
          // volta ao início após breve atraso quando chega ao final
          setTimeout(() => linha.scrollTo({ left: 0, behavior: 'smooth' }), 700);
        }
      }, intervalMs);
    };
    const stopAuto = () => {
      if (!auto) return;
      clearInterval(auto);
      auto = null;
    };

    carrossel.addEventListener('mouseenter', stopAuto);
    carrossel.addEventListener('mouseleave', startAuto);
    carrossel.addEventListener('focusin', stopAuto);
    carrossel.addEventListener('focusout', startAuto);

    // inicia
    startAuto();
  });
});