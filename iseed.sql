 -- =================================================================
--  Seed para la base de datos de Inmobiliaria
--  Ordenado para una inserción simple y sin errores de FK.
-- =================================================================

-- 1. Tabla: tipo_inmueble (Sin dependencias)
-- =================================================================
INSERT INTO tipo_inmueble (nombre) VALUES
('Casa'),
('Departamento'),
('Local'),
('Oficina'),
('Galpon'),
('Hotel'),
('Cabaña'),
('Edificio'),
('Cochera'),
('Terreno');


-- 2. Tabla: propietario (Sin dependencias)
-- =================================================================
INSERT INTO `propietario` (`dni`, `nombre`, `apellido`, `telefono`, `email`, `direccion`) VALUES
('11222333', 'Carlos', 'Gomez', '2664558877', 'carlos.gomez@email.com', 'Avenida Principal 123'),
('22333444', 'Ana', 'Martinez', '2664990011', 'ana.martinez@email.com', 'Calle Secundaria 456'),
('33444555', 'Luis', 'Fernandez', '2664112233', 'luis.fernandez@email.com', 'Pasaje del Sol 789');

-- 3. Tabla: inquilino (Sin dependencias)
-- =================================================================
INSERT INTO `inquilino` (`dni`, `nombre`, `apellido`, `telefono`, `email`, `direccion`) VALUES
('44555666', 'Maria', 'Rodriguez', '2657334455', 'maria.r@email.com', 'Boulevard de los Arboles 101'),
('55666777', 'Jorge', 'Perez', '2657889900', 'jorge.perez@email.com', 'Avenida Norte 212'),
('66777888', 'Sofia', 'Lopez', '2657665544', 'sofia.lopez@email.com', 'Calle Sur 333');

-- 4. Tabla: inmueble (Depende de propietario y tipo_inmueble)
-- =================================================================
-- Inmueble de Carlos Gomez (id_propietario=1)
INSERT INTO `inmueble` (`id_propietario`, `id_tipo_inmueble`, `direccion`, `uso`, `cantidad_ambientes`, `longitud`, `latitud`, `precio`, `estado`, `descripcion`) VALUES
(1, 2, 'San Martin 550, Piso 2, Dpto A', 1, 3, '-66.335634', '-33.302008', 85000.00, 1, 'Departamento centrico con 2 habitaciones'),
(1, 1, 'Rivadavia 1250', 1, 4, '-66.319876', '-33.295432', 120000.00, 1, 'Casa amplia con patio y cochera'),
-- Inmueble de Ana Martinez (id_propietario=2)
(2, 3, 'Junin 980', 2, 2, '-66.338123', '-33.305678', 150000.00, 1, 'Local comercial a la calle, zona transitada'),
-- Inmueble de Luis Fernandez (id_propietario=3)
(3, 2, 'Falucho 2100', 1, 2, '-66.301122', '-33.318899', 70000.00, 2, 'Departamento de 1 dormitorio, suspendido temporalmente');

-- 5. Tabla: contrato (Depende de inquilino e inmueble)
-- =================================================================
-- Contrato para el departamento céntrico (inmueble 1) con Maria Rodriguez (inquilino 1)
INSERT INTO `contrato` (`id_inquilino`, `id_inmueble`, `fecha_desde`, `fecha_hasta`, `monto_mensual`, `tipo`) VALUES
(1, 1, '2024-08-01', '2026-07-31', 85000.00, 2);

-- Contrato para el local comercial (inmueble 3) con Jorge Perez (inquilino 2)
INSERT INTO `contrato` (`id_inquilino`, `id_inmueble`, `fecha_desde`, `fecha_hasta`, `monto_mensual`, `multa`, `tipo`) VALUES
(2, 3, '2025-01-15', '2028-01-14', 150000.00, 75000.00, 2);

-- Contrato finalizado para la casa (inmueble 2) con Sofia Lopez (inquilino 3)
INSERT INTO `contrato` (`id_inquilino`, `id_inmueble`, `fecha_desde`, `fecha_hasta`, `fecha_terminacion_anticipada`, `monto_mensual`, `tipo`) VALUES
(3, 2, '2023-05-01', '2025-04-30', '2024-10-15', 110000.00, 2);


-- 6. Tabla: pago (Depende de contrato)
-- =================================================================
-- Pagos para el Contrato 1 (Maria Rodriguez en Dpto Céntrico)
INSERT INTO `pago` (`id_contrato`, `numero_pago`, `fecha_pago`, `concepto`, `monto`) VALUES
(1, 1, '2024-08-05', 'Pago alquiler Agosto 2024', 85000.00),
(1, 2, '2024-09-04', 'Pago alquiler Septiembre 2024', 85000.00),
(1, 3, '2024-10-06', 'Pago alquiler Octubre 2024', 85000.00);

-- Pagos para el Contrato 2 (Jorge Perez en Local Comercial)
INSERT INTO `pago` (`id_contrato`, `numero_pago`, `fecha_pago`, `concepto`, `monto`) VALUES
(2, 1, '2025-01-14', 'Pago alquiler Enero 2025', 150000.00),
(2, 2, '2025-02-10', 'Pago alquiler Febrero 2025', 150000.00);

-- Un pago anulado para el Contrato 2
INSERT INTO `pago` (`id_contrato`, `numero_pago`, `fecha_pago`, `concepto`, `monto`, `estado`) VALUES
(2, 3, '2025-03-11', 'Transferencia rechazada', 150000.00, 2);

-- Pagos para el Contrato 3 (Finalizado)
INSERT INTO `pago` (`id_contrato`, `numero_pago`, `fecha_pago`, `concepto`, `monto`) VALUES
(3, 1, '2023-05-03', 'Pago alquiler Mayo 2023', 110000.00),
(3, 2, '2023-06-05', 'Pago alquiler Junio 2023', 110000.00);