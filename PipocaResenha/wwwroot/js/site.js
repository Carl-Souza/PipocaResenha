// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

<script>
        document.querySelectorAll('.carrossel-filmes').forEach(carrossel => {
          const linha = carrossel.querySelector('.filme-linha');
    const esquerda = carrossel.querySelector('.seta-esquerda');
    const direita = carrossel.querySelector('.seta-direita');
    const scrollStep = 400;
          esquerda.addEventListener('click', () => {
        linha.scrollBy({ left: -scrollStep, behavior: 'smooth' });
          });
          direita.addEventListener('click', () => {
        linha.scrollBy({ left: scrollStep, behavior: 'smooth' });
          });
          // Rolagem automática
          let autoScroll = setInterval(() => {
        linha.scrollBy({ left: scrollStep, behavior: 'smooth' });
            if (linha.scrollLeft + linha.clientWidth >= linha.scrollWidth) {
        linha.scrollTo({ left: 0, behavior: 'smooth' });
            }
          }, 4000);
          carrossel.addEventListener('mouseenter', () => clearInterval(autoScroll));
          carrossel.addEventListener('mouseleave', () => {
        autoScroll = setInterval(() => {
            linha.scrollBy({ left: scrollStep, behavior: 'smooth' });
            if (linha.scrollLeft + linha.clientWidth >= linha.scrollWidth) {
                linha.scrollTo({ left: 0, behavior: 'smooth' });
            }
        }, 4000);
          });
        });
</script>