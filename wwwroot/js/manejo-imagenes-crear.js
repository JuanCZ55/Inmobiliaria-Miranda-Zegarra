function inicializarGestionImagenes(config) {
  // --- SELECTORES ---
  const inputPortada = document.getElementById(config.idInputPortada);
  const previewPortada = document.getElementById(config.idPreviewPortada);
  const inputGaleria = document.getElementById(config.idInputGaleria);
  const carouselInner = document.getElementById(config.idCarouselInner);
  const carousel = document.getElementById(config.idCarousel);

  // Validamos que todos los elementos existan
  if (
    !inputPortada ||
    !previewPortada ||
    !inputGaleria ||
    !carouselInner ||
    !carousel
  ) {
    console.error(
      "Faltan uno o más elementos del DOM para inicializar la gestión de imágenes."
    );
    return;
  }

  // Este array almacenará los archivos de la galería temporalmente
  let archivosGaleria = [];

  // ===================================
  // FUNCIÓN PARA GESTIONAR LA PORTADA
  // ===================================
  const manejarPortada = (event) => {
    previewPortada.innerHTML = ""; // Limpiamos la previsualización anterior
    const file = event.target.files[0];

    if (!file || !file.type.startsWith("image/")) {
      inputPortada.value = ""; // Limpia por si se seleccionó un archivo no válido
      return;
    }

    const imgUrl = URL.createObjectURL(file);
    const div = document.createElement("div");
    // --- LÍNEA MODIFICADA ---
    div.className = "position-relative"; // Quitamos d-inline-block

    const img = document.createElement("img");
    img.src = imgUrl;
    img.className = "d-block w-100"; // Esta clase ahora funcionará como en la galería
    img.style.height = "300px";
    img.style.objectFit = "cover";

    const btn = document.createElement("button");
    btn.innerHTML = "&times;";
    btn.className = "btn btn-danger btn-sm position-absolute top-0 end-0 m-2";
    btn.style.zIndex = "10";
    btn.type = "button"; // Para evitar que envíe el formulario

    btn.addEventListener("click", () => {
      inputPortada.value = ""; // ¡SOLUCIÓN! Limpia el input del formulario.
      URL.revokeObjectURL(imgUrl);
      div.remove();
    });

    div.appendChild(img);
    div.appendChild(btn);
    previewPortada.appendChild(div);
  };
  // ===================================
  // FUNCIONES PARA GESTIONAR LA GALERÍA
  // ===================================
  const manejarGaleria = (event) => {
    const nuevosArchivos = Array.from(event.target.files);
    nuevosArchivos.forEach((file) => {
      if (file.type.startsWith("image/")) {
        // Evitamos duplicados por si el usuario selecciona el mismo archivo
        if (
          !archivosGaleria.some(
            (f) => f.name === file.name && f.size === file.size
          )
        ) {
          archivosGaleria.push(file);
        }
      }
    });
    renderizarGaleria();
  };

  const renderizarGaleria = () => {
    carouselInner.innerHTML = ""; // Limpiamos para redibujar

    archivosGaleria.forEach((file, idx) => {
      const div = document.createElement("div");
      div.className = `carousel-item ${idx === 0 ? "active" : ""}`;

      const img = document.createElement("img");
      img.src = URL.createObjectURL(file);
      img.className = "d-block w-100";
      img.style.height = "300px";
      img.style.objectFit = "cover";

      const btn = document.createElement("button");
      btn.innerHTML = "&times;";
      btn.className = "btn btn-danger btn-sm position-absolute top-0 end-0 m-2";
      btn.style.zIndex = "10";
      btn.type = "button";

      btn.addEventListener("click", () => {
        // Liberamos memoria del objeto URL antes de quitarlo
        URL.revokeObjectURL(img.src);
        // Filtramos el array para eliminar el archivo
        archivosGaleria = archivosGaleria.filter((f) => f !== file);
        // Volvemos a renderizar todo
        renderizarGaleria();
      });

      div.appendChild(img);
      div.appendChild(btn);
      carouselInner.appendChild(div);
    });

    // ¡SOLUCIÓN! Actualizamos el input con todos los archivos acumulados.
    // Usamos el objeto DataTransfer para crear una nueva FileList.
    const dataTransfer = new DataTransfer();
    archivosGaleria.forEach((file) => dataTransfer.items.add(file));
    inputGaleria.files = dataTransfer.files;

    // Ocultamos los controles del carrusel si hay 1 o 0 imágenes
    carousel.style.display = archivosGaleria.length > 0 ? "block" : "none";
    const controls = carousel.querySelectorAll(
      ".carousel-control-prev, .carousel-control-next"
    );
    controls.forEach((control) => {
      control.style.display = archivosGaleria.length > 1 ? "flex" : "none";
    });
  };

  // --- EVENT LISTENERS ---
  inputPortada.addEventListener("change", manejarPortada);
  inputGaleria.addEventListener("change", manejarGaleria);
}
