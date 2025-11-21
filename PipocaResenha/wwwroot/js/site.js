document.addEventListener("DOMContentLoaded", () => {
    const carrosseis = document.querySelectorAll('.carrossel-filmes');

    carrosseis.forEach(container => {
        const linha = container.querySelector('.filme-linha');
        const btnEsq = container.querySelector('.seta-esquerda');
        const btnDir = container.querySelector('.seta-direita');

        if (!linha || !btnEsq || !btnDir) return;

        const scrollAmount = 450;

        btnDir.addEventListener('click', () => {
            linha.scrollBy({ left: scrollAmount, behavior: 'smooth' });
        });

        btnEsq.addEventListener('click', () => {
            linha.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
        });
    });
});