document.addEventListener('DOMContentLoaded', function () {
  // cria modal único para trailers
  if (!document.getElementById('tmdb-trailer-modal')) {
    const modalHtml = `
      <div class="modal fade" id="tmdb-trailer-modal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title" id="tmdb-trailer-title"></h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
              <div id="tmdb-trailer-container" style="position:relative;padding-top:56.25%"></div>
            </div>
          </div>
        </div>
      </div>`;
    document.body.insertAdjacentHTML('beforeend', modalHtml);
  }

  document.querySelectorAll('.carrossel-filmes').forEach(function (carousel) {
    const linha = carousel.querySelector('.filme-linha');
    if (!linha) return;
    if (linha.children.length > 0) return; // não sobrescreve conteúdo já existente

    const section = (carousel.dataset.section || carousel.previousElementSibling?.innerText || 'Lancamentos').trim();
    fetch(`/api/filmes/tmdb?section=${encodeURIComponent(section)}&count=10`)
      .then(r => r.ok ? r.json() : Promise.reject(r))
      .then(list => {
        linha.innerHTML = list.map(m => {
          const poster = m.posterUrl || 'https://via.placeholder.com/300x450?text=Sem+Imagem';
          return `
            <div class="filme-card">
              <img src="${poster}" alt="${(m.titulo||'')}" />
              <div class="filme-info">
                <h5>${(m.titulo||'')}</h5>
                <p>${(m.sinopseCurta||'')}</p>
                <div class="filme-botoes">
                  <a class="btn btn-sm btn-primary" href="${m.detailsUrl}">Ver mais</a>
                  <button data-tmdb-id="${m.codigo}" class="btn btn-sm btn-secondary btn-trailer">Trailer</button>
                  <button class="btn btn-sm btn-outline-secondary">+ Lista</button>
                </div>
              </div>
            </div>`;
        }).join('');

        // associar evento dos trailers
        linha.querySelectorAll('.btn-trailer').forEach(btn => {
          btn.addEventListener('click', function () {
            const id = this.dataset.tmdbId;
            if (!id) return;
            fetch(`/api/filmes/tmdb/details/${id}`)
              .then(r => r.ok ? r.json() : Promise.reject(r))
              .then(d => {
                const videos = (d.videos || []).filter(v => v.site === 'YouTube' && v.type && v.type.toLowerCase().includes('trailer'));
                const titleEl = document.getElementById('tmdb-trailer-title');
                const container = document.getElementById('tmdb-trailer-container');
                titleEl.textContent = d.titulo || '';
                if (videos.length === 0) {
                  container.innerHTML = '<p>Trailer não disponível.</p>';
                } else {
                  const key = videos[0].key;
                  container.innerHTML = `<iframe src="https://www.youtube.com/embed/${key}?autoplay=1" style="position:absolute;left:0;top:0;width:100%;height:100%;" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>`;
                }
                const modal = new bootstrap.Modal(document.getElementById('tmdb-trailer-modal'));
                modal.show();
              })
              .catch(err => {
                console.error('Erro ao obter detalhes TMDB', err);
              });
          });
        });
      })
      .catch(err => console.error('Erro ao carregar carrossel TMDB', err));
  });
});