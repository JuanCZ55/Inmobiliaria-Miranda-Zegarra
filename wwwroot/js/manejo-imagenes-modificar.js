/**
 * Marca una imagen existente para ser eliminada.
 * Oculta la imagen y crea un input hidden con su ID para enviarlo al controlador.
 * @param {number} id - El ID de la imagen a eliminar.
 * @param {string} containerId - El ID del div que contiene la imagen y el botón.
 */
function marcarParaEliminar(id, containerId) {
  // Ocultar visualmente la imagen para que el usuario vea la acción
  const imgContainer = document.getElementById(containerId);
  if (imgContainer) {
    imgContainer.style.display = "none";
  }

  // Crear un input hidden para enviar el ID al controlador
  const hiddenInputsContainer = document.getElementById(
    "container-eliminar-ids"
  );
  const input = document.createElement("input");
  input.type = "hidden";
  input.name = "EliminarIDs"; // Debe coincidir con la propiedad del modelo Inmueble
  input.value = id;
  hiddenInputsContainer.appendChild(input);
}

/**
 * Inicializa la previsualización de nuevas imágenes en el formulario de modificación.
 * @param {string} idInputPortada - El ID del input file para la nueva portada.
 * @param {string} idPreviewPortada - El ID del div para la previsualización de la nueva portada.
 * @param {string} idInputGaleria - El ID del input file para las nuevas imágenes de galería.
 * @param {string} idPreviewGaleria - El ID del div para la previsualización de la nueva galería.
 */
function inicializarPreviewsModificar(config) {
  const inputPortada = document.getElementById(config.idInputPortada);
  const previewPortada = document.getElementById(config.idPreviewPortada);
  const inputGaleria = document.getElementById(config.idInputGaleria);
  const previewGaleria = document.getElementById(config.idPreviewGaleria);

  if (!inputPortada || !previewPortada || !inputGaleria || !previewGaleria) {
    console.error("Faltan elementos del DOM para las previsualizaciones.");
    return;
  }

  // Previsualización para la nueva portada
  inputPortada.addEventListener("change", (event) => {
    previewPortada.innerHTML = ""; // Limpiar preview anterior
    const file = event.target.files[0];
    if (file && file.type.startsWith("image/")) {
      const reader = new FileReader();
      reader.onload = (e) => {
        const img = document.createElement("img");
        img.src = e.target.result;
        img.className = "img-thumbnail";
        img.style.maxHeight = "200px";
        previewPortada.appendChild(img);
      };
      reader.readAsDataURL(file);
    }
  });

  // Previsualización para las nuevas imágenes de galería
  inputGaleria.addEventListener("change", (event) => {
    previewGaleria.innerHTML = ""; // Limpiar previews anteriores
    Array.from(event.target.files).forEach((file) => {
      if (file.type.startsWith("image/")) {
        const reader = new FileReader();
        reader.onload = (e) => {
          const img = document.createElement("img");
          img.src = e.target.result;
          img.className = "img-thumbnail";
          img.style.width = "120px";
          img.style.height = "120px";
          img.style.objectFit = "cover";
          previewGaleria.appendChild(img);
        };
        reader.readAsDataURL(file);
      }
    });
  });
}
