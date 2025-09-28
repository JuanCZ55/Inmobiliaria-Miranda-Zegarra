// Variable para inicializar el mapa y Select2
function inicializarFormularioInmueble(modelData) {
  // === Select2 Propietario ===
  $("#select-propietario").select2({
    theme: "bootstrap-5",
    placeholder: "Busque un propietario...",
    minimumInputLength: 3,
    ajax: {
      url: "/Propietario/ListarPropietarios",
      dataType: "json",
      delay: 250,
      data: (params) => ({ q: params.term }),
      processResults: (data) => ({
        results: $.map(data, (item) => ({
          id: item.idPropietario,
          text: item.nombre + " " + item.apellido + " (" + item.dni + ")",
        })),
      }),
    },
  });

  // Si hay datos de un propietario, lo inicializamos
  if (modelData && modelData.idPropietario) {
    const option = new Option(
      modelData.propietarioNombre,
      modelData.idPropietario,
      true,
      true
    );
    $("#select-propietario").append(option).trigger("change");
  }

  // === MAPA + DIRECCIÓN ===
  const $dir = $("#direccion");
  const $lat = $("#latitud");
  const $lng = $("#longitud");
  const $list = $("#addrList");

  const DEF_LAT = -33.30216,
    DEF_LNG = -66.336993;
  const initLat = parseFloat($lat.val()) || DEF_LAT;
  const initLng = parseFloat($lng.val()) || DEF_LNG;

  const map = L.map("mapa").setView([initLat, initLng], 15);
  L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
    attribution: "© OpenStreetMap contributors",
  }).addTo(map);
  const marker = L.marker([initLat, initLng], { draggable: true }).addTo(map);

  // actualizar inputs readonly
  function setInputs(lat, lng) {
    $lat.val(lat.toFixed(6));
    $lng.val(lng.toFixed(6));
  }
  setInputs(initLat, initLng);

  // reverse geocoding
  async function reverseGeo(lat, lon) {
    try {
      const r = await fetch(
        `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lat}&lon=${lon}`,
        {
          headers: { "Accept-Language": "es" },
        }
      );
      const j = await r.json();
      return j.display_name || "";
    } catch {
      return "";
    }
  }

  // búsqueda de direcciones
  async function searchAddr(q) {
    try {
      const r = await fetch(
        `https://nominatim.openstreetmap.org/search?format=jsonv2&limit=5&q=${encodeURIComponent(
          q
        )}`,
        {
          headers: { "Accept-Language": "es" },
        }
      );
      return await r.json();
    } catch {
      return [];
    }
  }

  function showList(items) {
    $list.empty();
    if (!items.length) {
      $list.hide();
      return;
    }

    items.forEach((it) => {
      const $li = $("<li>")
        .text(it.display_name)
        .data({ lat: it.lat, lon: it.lon });
      $li.on("click", function () {
        const lat = parseFloat($(this).data("lat")),
          lon = parseFloat($(this).data("lon"));
        $dir.val(it.display_name);
        marker.setLatLng([lat, lon]);
        map.setView([lat, lon], 15);
        setInputs(lat, lon);
        $list.hide();
      });
      $list.append($li);
    });
    $list.show();
  }

  const debounce = (fn, ms = 300) => {
    let t;
    return (...a) => {
      clearTimeout(t);
      t = setTimeout(() => fn(...a), ms);
    };
  };

  const doSuggest = debounce(async (q) => {
    if (q.trim().length < 2) {
      $list.hide();
      return;
    }
    showList(await searchAddr(q));
  }, 300);

  $dir.on("input", () => doSuggest($dir.val()));

  $(document).on("click", (e) => {
    if (!$(e.target).closest(".position-relative").length) $list.hide();
  });

  // mover marcador
  marker.on("dragend", async (e) => {
    const p = e.target.getLatLng();
    setInputs(p.lat, p.lng);
    $dir.val(await reverseGeo(p.lat, p.lng));
  });

  map.on("click", async (e) => {
    marker.setLatLng(e.latlng);
    map.panTo(e.latlng);
    setInputs(e.latlng.lat, e.latlng.lng);
    $dir.val(await reverseGeo(e.latlng.lat, e.latlng.lng));
  });
}
