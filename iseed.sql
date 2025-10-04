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


-- 7. Tabla: usuario (Sin dependencias)
-- =================================================================
INSERT INTO `usuario` (`email`, `contrasena`, `rol`, `nombre`, `apellido`, `url`, `estado`, `created_at`, `updated_at`, `genero`) VALUES
('user01@gmail.com', 'AQAAAAIAAYagAAAAEJe/0FDlu1A56hCXRObw/IQIVvBwuRHpO/rY+VOxw8ovSuBcExD08VeAUC/FVGygHw==', 1, 'El Hechizero', 'El Gran Hechizero', NULL, 1, '2025-10-02 14:49:00', '2025-10-04 01:17:20', 1),
('user02@gmail.com', 'AQAAAAIAAYagAAAAEKg5Vve4ODlWkm+bML/CpVDBgCCwaE0JRfUgu5GKAVMSLpzMzJkSxf4UAEBnZJxRTw==', 2, 'Juan', 'Garcia', NULL, 1, '2025-10-04 01:31:48', '2025-10-04 01:31:48', 1),
('user03@gmail.com', 'AQAAAAIAAYagAAAAEE4RSc+lFxiacOdRC0dmOZGAPGngKGUtxz1yfVlunUvKihOwVunD3n63YRq5m2CiAg==', 2, 'Sofia', 'Rodriguez', NULL, 1, '2025-10-04 01:32:55', '2025-10-04 01:32:55', 2),
('user04@gmail.com', 'AQAAAAIAAYagAAAAENi6EsSJK3eWU2gni/PR/GdRdV9k3cgO0peJ/jTPLYJbVGWvKh/ea/4urpcarMFZQg==', 2, 'Carlos', 'Martinez', NULL, 1, '2025-10-04 01:34:11', '2025-10-04 01:34:11', 1),
('user05@gmail.com', 'AQAAAAIAAYagAAAAELncCUmdWjq1ae0t1xiHyVfJuvzJlvEiZecwBAFIrjJ5h7CmzrKSoLKUu0/L2A6EnA==', 2, 'Valentina', 'Hernandez', NULL, 1, '2025-10-04 01:35:03', '2025-10-04 01:35:03', 2),
('user06@gmail.com', 'AQAAAAIAAYagAAAAEPCx0NX6maj8ZF1fLtwdtjZ7ztdqzeT7qXy8FAXt4lEPL/p8z7dxPjCeN8DTbY2HQQ==', 2, 'Luis', 'Lopez', NULL, 1, '2025-10-04 01:35:32', '2025-10-04 01:35:32', 1),
('user07@gmail.com', 'AQAAAAIAAYagAAAAEO9qvgaouHKcMv0HLeq+bvPi23Ur+x/rDLcj4BpZpQVIrhEViVUgMPCxrmpvBBt2UA==', 2, 'Camila', 'Gonzales', NULL, 1, '2025-10-04 01:35:59', '2025-10-04 01:35:59', 2),
('user08@gmail.com', 'AQAAAAIAAYagAAAAEB4Lw83Vn88KKbquynyiaLnz8DSAtBbLtN2sTHs8tEX9ENDoviSvpY7+Ep9VSHYJHg==', 2, 'Javier', 'Perez', NULL, 1, '2025-10-04 01:36:30', '2025-10-04 01:36:30', 1),
('user09@gmail.com', 'AQAAAAIAAYagAAAAELOS6i4FKEHEOy5mUMS5A30BMIDN+9nyiBKz76Y6dJmI8pyN+KIXrflwgDbWq0+Qgw==', 2, 'Isabella', 'Sanchez', NULL, 1, '2025-10-04 01:36:49', '2025-10-04 01:36:49', 2),
('user11@gmail.com', 'AQAAAAIAAYagAAAAEDh9nCvxfMWSMWYfKKY/iiiMCbrwoUpikDIghDmdjUdoWMoOBEpMF75ugG48A+XYEQ==', 2, 'Diego', 'Ramirez', NULL, 1, '2025-10-04 01:37:15', '2025-10-04 01:37:15', 1),
('user12@gmail.com', 'AQAAAAIAAYagAAAAEIXrpNMGUtrPmiyZYXNqMwczr3Lvnga/lSLgRO+6YdjXxmxt21V7mnfZ7sLUV//0jw==', 2, 'Ana', 'Torres', NULL, 1, '2025-10-04 01:37:37', '2025-10-04 01:37:37', 2),
('user13@gmail.com', 'AQAAAAIAAYagAAAAEOvHtIYqIszqPqsXd0XTB1EBTsIA+EIMXMSt1bypM2Xmjpi/80cj4GdLShtbv/nmnw==', 2, 'Mateo', 'Flores', NULL, 1, '2025-10-04 01:37:58', '2025-10-04 01:37:58', 1),
('user14@gmail.com', 'AQAAAAIAAYagAAAAEN8zZiBDG2J3efT5l7rqMHyfDzwzQ8RFE9dUM8hj/zmx/GpDiSDW6pzU22abo3xsYA==', 2, 'Laura', 'Rivera', NULL, 1, '2025-10-04 01:38:27', '2025-10-04 01:38:27', 2),
('user15@gmail.com', 'AQAAAAIAAYagAAAAEO7kp91H1uXNrTcWoJK40t6rUtmT26xKpsYq/0HudkD6nYR5pxbbPgmsx1ccL4p37g==', 2, 'Miguel', 'Gomez', NULL, 1, '2025-10-04 01:39:13', '2025-10-04 01:39:13', 1),
('user16@gmail.com', 'AQAAAAIAAYagAAAAEIZNWR1VFdlaJEubeEHJJpA9eEl/NyHUssd4U8S7oF/Qb5Wh/fHrUQFV0cj8MEUffQ==', 2, 'Maria', 'Diaz', NULL, 1, '2025-10-04 01:39:34', '2025-10-04 01:39:34', 2),
('user17@gmail.com', 'AQAAAAIAAYagAAAAEHRKYN6fiUpGgbyFsqmU5tCn0+7VNe4qRwYyAMaeA2o2s0tNPAmlL1twPwUffBJPjw==', 2, 'Alejandro', 'Cruz', NULL, 1, '2025-10-04 01:39:58', '2025-10-04 01:39:58', 1),
('user18@gmail.com', 'AQAAAAIAAYagAAAAECwRo+l13Dj/n4irMrReU7gKH/HjjpbrBeiNNdrOKweEQ/vkym41UUd9VRhITP7BvQ==', 1, 'Pato', 'Lucas', NULL, 1, '2025-10-04 01:40:18', '2025-10-04 01:40:18', 1);
