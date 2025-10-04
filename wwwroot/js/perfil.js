let originalAvatarURL = '';

function mostrarAlerta(message, type = 'info') {
    const alertContainer = document.getElementById('alert-container');
    if (!alertContainer) {
        console.error('El contenedor #alert-container no existe.');
        return; 
    }

    const alertElement = document.createElement('div');
    alertElement.className = `alert alert-${type} alertaTemp shadow mt-3 w-75 mx-auto`;
    alertElement.setAttribute('role', 'alert');
    alertElement.style.pointerEvents = 'auto';
    alertElement.textContent = message;
    
    alertContainer.append(alertElement);
    setTimeout(() => alertElement.remove(), 5000); 
}

let originalProfileData = {};

async function cargarPerfil(modalElement) {
  try {
    const form = modalElement.querySelector('#profileForm');

    const response = await fetch('/Usuario/Perfil', {
      method: "GET",
      credentials: "include"
    });

    if (!response.ok) {
      mostrarAlerta("No se pudo cargar el perfil.", "danger");
      return;
    }

    const user = await response.json();

    originalProfileData = {
      nombre: user.nombre || "",
      apellido: user.apellido || "",
      email: user.email || "",
      genero: String(user.genero)
    };
    form.querySelector('#profileIdUsuario').value = user.idUsuario || "";
    form.querySelector('#profileNombre').value = user.nombre || "";
    form.querySelector('#profileApellido').value = user.apellido || "";
    form.querySelector('#profileEmail').value = user.email || "";

    const generoSelect = form.querySelector('#profileGenero');
    if (generoSelect) {
      generoSelect.value = String(user.genero);
    }

    const defaultMaleAvatar = modalElement.dataset.maleAvatar;
    const defaultFemaleAvatar = modalElement.dataset.femaleAvatar;

    const avatarPreview = form.querySelector('#avatarPreview');
    originalAvatarURL = user.avatarURL; // Asignamos la URL original
    avatarPreview.src = originalAvatarURL;
    form.querySelector('#profileAvatarURL').value = originalAvatarURL;

    const cancelAvatarButton = form.querySelector('#cancelAvatarChange');
    const isDefaultAvatar = avatarPreview.src.endsWith(defaultMaleAvatar) || avatarPreview.src.endsWith(defaultFemaleAvatar);

    // Si el avatar NO es el predeterminado, mostramos el botón para quitarlo.
    if (!isDefaultAvatar) {
      cancelAvatarButton.classList.remove('d-none');
    } else {
      cancelAvatarButton.classList.add('d-none');
    }
    // Deshabilitamos el botón de guardar al cargar, ya que no hay cambios.
    form.querySelector('button[type="submit"]').disabled = true;

  } catch (error) {
    console.error('Error en la petición para obtener el perfil:', error);
    mostrarAlerta("Error de red al cargar el perfil.", "danger");
  }
}


document.addEventListener('DOMContentLoaded', function () {
  const profileModal = document.getElementById('profileModal');
  if (profileModal) {
    // cargar perfil al abrir el modal
    profileModal.addEventListener('shown.bs.modal', function () {
      cargarPerfil(profileModal);
    });

    const profileForm = profileModal.querySelector('#profileForm');
    const passwordForm = profileModal.querySelector('#passwordForm');

    if (profileForm) {
      const submitButton = profileForm.querySelector('button[type="submit"]');
      const fileInput = profileForm.querySelector('#profileAvatar');
      const avatarPreview = profileForm.querySelector('#avatarPreview');
      const cancelAvatarButton = profileForm.querySelector('#cancelAvatarChange');
      const generoSelect = profileForm.querySelector('#profileGenero');
      const defaultMaleAvatar = profileModal.dataset.maleAvatar;
      const defaultFemaleAvatar = profileModal.dataset.femaleAvatar;

      fileInput.addEventListener('change', function () {
        const file = fileInput.files[0];
        if (file) {
          avatarPreview.src = URL.createObjectURL(file);
          cancelAvatarButton.classList.remove('d-none');
        }
      });

      // Lógica del botón "Quitar"
      cancelAvatarButton.addEventListener('click', function() {
        const avatarUrlInput = profileForm.querySelector('#profileAvatarURL');
        if (fileInput.files.length > 0) {
          fileInput.value = '';
          avatarPreview.src = originalAvatarURL;
          avatarUrlInput.value = originalAvatarURL;
          const isDefault = originalAvatarURL.endsWith(defaultMaleAvatar) || originalAvatarURL.endsWith(defaultFemaleAvatar);
          cancelAvatarButton.classList.toggle('d-none', isDefault);
        } else {
          avatarUrlInput.value = "-1";
          avatarPreview.src = (generoSelect.value === '2') ? defaultFemaleAvatar : defaultMaleAvatar;
          cancelAvatarButton.classList.add('d-none');
        }
        checkChanges();
      });

      // Cambiar avatar por defecto según el género
      generoSelect.addEventListener('change', function() {
        if (fileInput.files.length > 0) return;
        const isDefault = avatarPreview.src.endsWith(defaultMaleAvatar) || avatarPreview.src.endsWith(defaultFemaleAvatar);
        if (isDefault) {
          avatarPreview.src = (generoSelect.value === '2') ? defaultFemaleAvatar : defaultMaleAvatar;
        }
      });

      // Función para comprobar si hay cambios y habilitar/deshabilitar el botón
      const checkChanges = () => {
        const currentData = {
          nombre: profileForm.querySelector('#profileNombre').value,
          apellido: profileForm.querySelector('#profileApellido').value,
          email: profileForm.querySelector('#profileEmail').value,
          genero: profileForm.querySelector('#profileGenero').value
        };
        const avatarFile = fileInput.files[0];
        const avatarUrlInput = profileForm.querySelector('#profileAvatarURL').value;

        const hasChanged =
          currentData.nombre !== originalProfileData.nombre ||
          currentData.apellido !== originalProfileData.apellido ||
          currentData.email !== originalProfileData.email ||
          currentData.genero !== originalProfileData.genero ||
          avatarUrlInput !== originalAvatarURL ||
          !!avatarFile;

        submitButton.disabled = !hasChanged;
      };

      profileForm.addEventListener('input', checkChanges);

      profileForm.addEventListener('submit', async function (event) {
        event.preventDefault();

        const formData = new FormData(profileForm);
        const originalButtonText = submitButton.innerHTML;
        submitButton.disabled = true;
        submitButton.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Guardando...`;

        try {
          const response = await fetch('/Usuario/EditarPerfil', {
            method: 'POST',
            body: formData,
            credentials: "include",
          });

          if (response.ok) {
            const result = await response.json();
            const avatarHeader = document.querySelector('#userDropdown img');
            if (avatarHeader && result.newAvatarUrl) avatarHeader.src = result.newAvatarUrl;

            mostrarAlerta('Perfil actualizado correctamente.', 'success');
            
            originalProfileData = {
              nombre: formData.get('Nombre'),
              apellido: formData.get('Apellido'),
              email: formData.get('Email'),
              genero: formData.get('Genero')
            };
            originalAvatarURL = result.newAvatarUrl;
            profileForm.querySelector('#profileAvatarURL').value = result.newAvatarUrl;
            fileInput.value = '';

            checkChanges();

          } else {
            const error = await response.json();
            mostrarAlerta(`Error: ${error.message}`, 'danger');
          }
        } catch (error) {
          mostrarAlerta('Ocurrió un error de red al intentar actualizar el perfil.', 'danger');
        } finally {
          submitButton.disabled = false;
          submitButton.innerHTML = originalButtonText;
          checkChanges();
        }
      });
    }

    if (passwordForm) {
    const newPasswordInput = document.getElementById('profileNewPassword');
    const confirmPasswordInput = document.getElementById('profileConfirmPassword');
    const newPasswordErrorSpan = passwordForm.querySelector('[data-valmsg-for="profileNewPassword"]');
    const confirmPasswordErrorSpan = passwordForm.querySelector('[data-valmsg-for="profileConfirmPassword"]');

    // Función para validar las contraseñas
    const validatePasswords = () => {
      const newPassword = document.getElementById('profileNewPassword').value;
      const confirmPassword = document.getElementById('profileConfirmPassword').value;

      // Limpiar errores previos
      newPasswordErrorSpan.textContent = '';
      confirmPasswordErrorSpan.textContent = '';

      if (newPassword.length === 0 && confirmPassword.length === 0) {
        return true;
      }

      // Validación de longitud mínima
      if (newPassword.length > 0 && newPassword.length < 8) {
        newPasswordErrorSpan.textContent = 'La contraseña debe tener un mínimo de 8 caracteres.';
        return false;
      }

      // Validación de coincidencia
      if (newPassword !== confirmPassword) {
        newPasswordErrorSpan.textContent = 'Las contraseñas no coinciden.';
        confirmPasswordErrorSpan.textContent = 'Las contraseñas no coinciden.';
        return false;
      }

      return true;
    };

    newPasswordInput.addEventListener('input', validatePasswords);
    confirmPasswordInput.addEventListener('input', validatePasswords);

    passwordForm.addEventListener('submit', async function (event) {
      event.preventDefault();

      if (!validatePasswords()) return;

      const formData = new FormData(passwordForm);
      const submitButton = passwordForm.querySelector('button[type="submit"]');
      const originalButtonText = submitButton.innerHTML;
      submitButton.disabled = true;
      submitButton.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Actualizando...`;

      try {
        const response = await fetch('/Usuario/CambiarPassword', {
          method: 'POST',
          body: formData,
          credentials: "include",
        });

        const result = await response.json();

        if (response.ok) {
          mostrarAlerta(result.message, 'success');
          setTimeout(() => {
            passwordForm.reset();
          }, 1000);
        } else {
          mostrarAlerta(`Error: ${result.message}`, 'danger');
        }
      } catch (error) {
        mostrarAlerta('Ocurrió un error de red al intentar cambiar la contraseña.', 'danger');
      } finally {
        submitButton.disabled = false;
        submitButton.innerHTML = originalButtonText;
      }
    });

    profileModal.addEventListener('hidden.bs.modal', function () {
      passwordForm.reset();
    });
  }
  }
});