document.addEventListener('DOMContentLoaded', function () {
  document.querySelectorAll('.carrossel-filmes').forEach(car => {
    const section = car.dataset.section || 'Lancamentos';
    const linha = car.querySelector('.filme-linha');
    if (!linha) return;

    // spinner placeholder é filho atual; vamos substituir pelo conteúdo
    const spinner = linha.querySelector('.spinner-border') ? linha.querySelector('.spinner-border').closest('div') : null;

    fetch(`/api/filmes/tmdb?section=${encodeURIComponent(section)}&count=12`)
      .then(resp => {
        if (!resp.ok) throw resp;
        return resp.json();
      })
      .then(list => {
        if (spinner) spinner.remove();
        if (!list || !list.length) {
          linha.innerHTML = '<div class="d-flex justify-content-center w-100 py-5"><div class="text-muted">Nenhum filme encontrado.</div></div>';
          return;
        }
        linha.innerHTML = list.map(m => {
          const poster = m.posterUrl || 'https://via.placeholder.com/300x450?text=Sem+Imagem';
          const titulo = m.titulo || '';
          const sinopse = m.sinopseCurta || '';
          const details = m.detailsUrl || (`/Filmes/Details?codigo=${m.codigo || ''}`);
          return `<div class="card me-3" style="min-width:250px;">
                    <img src="${poster}" class="card-img-top" alt="${titulo}" />
                    <div class="card-body">
                      <h6>${titulo}</h6>
                      <p>${sinopse}</p>
                      <a class="btn btn-sm btn-primary" href="${details}">Ver</a>
                    </div>
                  </div>`;
        }).join('');
      })
      .catch(err => {
        if (spinner) spinner.remove();
        linha.innerHTML = '<div class="d-flex justify-content-center w-100 py-5"><div class="text-danger">Erro ao carregar filmes.</div></div>';
        console.error('Erro carregando carrossel', section, err);
      });
  });
});