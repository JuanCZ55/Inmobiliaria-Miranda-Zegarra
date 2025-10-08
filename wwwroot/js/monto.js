document.addEventListener("DOMContentLoaded", function () {
  const tipoPago = document.getElementById("Tipo");
  const fechaInicio = document.getElementById("fechaInicio");
  const fechaFin = document.getElementById("fechaFin");
  const precio = document.getElementById("precio");
  const monto = document.getElementById("Monto");
  const elementosAEscuchar = [tipoPago, fechaInicio, fechaFin, precio];

  function actualizarMonto() {
    const valorTipoPago = tipoPago.value;
    const valorFechaInicio = fechaInicio.value;
    const valorFechaFin = fechaFin.value;
    const valorPrecio = precio.value.trim();

    if (valorTipoPago && valorFechaInicio && valorFechaFin && valorPrecio) {
      const valorTipoPagoNum = parseInt(valorTipoPago, 10);
      const valorPrecioNum = parseFloat(valorPrecio);

      if (isNaN(valorPrecioNum)) return;
      const fecha1 = new Date(valorFechaInicio);
      const fecha2 = new Date(valorFechaFin);
      const diferenciaTiempo = fecha2.getTime() - fecha1.getTime();

      const dias = Math.max(0, diferenciaTiempo / (1000 * 60 * 60 * 24));

      let porcen = valorTipoPagoNum === 1 ? 1.4 : 1.2;
      let presi = valorPrecioNum / 30;

      monto.value = Math.round(dias * presi * porcen);
    } else {
      monto.value = "";
    }
  }

  elementosAEscuchar.forEach(function (elemento) {
    if (elemento) {
      elemento.addEventListener("change", actualizarMonto);
    }
  });
});
