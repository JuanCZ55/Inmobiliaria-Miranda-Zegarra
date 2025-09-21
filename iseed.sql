-- 1. Usuarios (10 registros)
INSERT INTO usuario (email, contrasena, rol, nombre, apellido, url, estado) VALUES
('admin1@inmob.com', '123456', 1, 'Juan', 'Perez', 'url1.jpg', 1),
('empleado1@inmob.com', '123456', 2, 'Maria', 'Gomez', 'url2.jpg', 1),
('empleado2@inmob.com', '123456', 2, 'Carlos', 'Lopez', 'url3.jpg', 1),
('empleado3@inmob.com', '123456', 2, 'Ana', 'Martinez', 'url4.jpg', 1),
('empleado4@inmob.com', '123456', 2, 'Pedro', 'Rodriguez', 'url5.jpg', 1),
('empleado5@inmob.com', '123456', 2, 'Laura', 'Sanchez', 'url6.jpg', 1),
('empleado6@inmob.com', '123456', 2, 'Diego', 'Fernandez', 'url7.jpg', 1),
('empleado7@inmob.com', '123456', 2, 'Sofia', 'Garcia', 'url8.jpg', 1),
('empleado8@inmob.com', '123456', 2, 'Miguel', 'Diaz', 'url9.jpg', 1),
('empleado9@inmob.com', '123456', 2, 'Elena', 'Ruiz', 'url10.jpg', 1);

-- 2. Tipo Inmueble (10 registros)
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

-- 3. Propietarios (10 registros)
INSERT INTO propietario (dni, nombre, apellido, telefono, email, direccion) VALUES
('12345678', 'Roberto', 'Silva', '111111111', 'roberto@mail.com', 'Calle Falsa 123'),
('23456789', 'Marta', 'Vega', '222222222', 'marta@mail.com', 'Av Siempre Viva 456'),
('34567890', 'Jorge', 'Molina', '333333333', 'jorge@mail.com', 'Pje Real 789'),
('45678901', 'Lucia', 'Rios', '444444444', 'lucia@mail.com', 'Camino Verde 321'),
('56789012', 'Oscar', 'Paz', '555555555', 'oscar@mail.com', 'Ruta Azul 654'),
('67890123', 'Isabel', 'Sola', '666666666', 'isabel@mail.com', 'Bulevar Rojo 987'),
('78901234', 'Francisco', 'Luna', '777777777', 'francisco@mail.com', 'Sendero Negro 147'),
('89012345', 'Eva', 'Costa', '888888888', 'eva@mail.com', 'Paseo Blanco 258'),
('90123456', 'Raul', 'Mar', '999999999', 'raul@mail.com', 'Via Gris 369'),
('01234567', 'Carmen', 'Sol', '000000000', 'carmen@mail.com', 'Avenida Dorada 159');

-- 4. Inquilinos (10 registros)
INSERT INTO inquilino (dni, nombre, apellido, telefono, email, direccion) VALUES
('11223344', 'Alberto', 'Mendez', '101010101', 'alberto@mail.com', 'Carrera 11 222'),
('22334455', 'Beatriz', 'Guerra', '202020202', 'beatriz@mail.com', 'Transversal 22 333'),
('33445566', 'Daniel', 'Roca', '303030303', 'daniel@mail.com', 'Diagonal 33 444'),
('44556677', 'Fabiola', 'Tapia', '404040404', 'fabiola@mail.com', 'Circunvalar 44 555'),
('55667788', 'Gabriel', 'Nunez', '505050505', 'gabriel@mail.com', 'Autopista 55 666'),
('66778899', 'Helena', 'Zambrano', '606060606', 'helena@mail.com', 'Corredor 66 777'),
('77889900', 'Ivan', 'Quintero', '707070707', 'ivan@mail.com', 'Viaducto 77 888'),
('88990011', 'Jessica', 'Orellana', '808080808', 'jessica@mail.com', 'Variante 88 999'),
('99001122', 'Kevin', 'Urbina', '909090909', 'kevin@mail.com', 'Anillo 99 000'),
('10011001', 'Natalia', 'Paredes', '121212121', 'natalia@mail.com', 'Periferico 10 111');

-- 5. Inmuebles (10 registros)
INSERT INTO inmueble (id_propietario, id_tipo_inmueble, direccion, uso, cantidad_ambientes, longitud, latitud, precio, descripcion) VALUES
(1, 1, 'Av Constructores 123', 1, 4, '-58.123', '-34.456', 150000.00, 'Casa amplia con jardin'),
(2, 2, 'Calle Altos 456', 1, 3, '-58.234', '-34.567', 120000.00, 'Departamento céntrico'),
(3, 3, 'Bulevar Comercial 789', 2, 2, '-58.345', '-34.678', 180000.00, 'Local en zona comercial'),
(4, 4, 'Pje Oficinas 321', 2, 5, '-58.456', '-34.789', 200000.00, 'Oficinas modernas'),
(5, 5, 'Ruta Industrial 654', 2, 1, '-58.567', '-34.890', 90000.00, 'Galpon logistico'),
(6, 6, 'Av Turistica 987', 2, 12, '-58.678', '-34.901', 350000.00, 'Hotel con piscina'),
(7, 7, 'Camino Serrano 147', 1, 3, '-58.789', '-34.012', 110000.00, 'Cabaña rustica'),
(8, 8, 'Torre Norte 258', 2, 20, '-58.890', '-34.123', 500000.00, 'Edificio corporativo'),
(9, 9, 'Estacionamiento 369', 2, 1, '-58.901', '-34.234', 60000.00, 'Cochera cubierta'),
(10, 10, 'Loteamiento Sur 159', 2, 0, '-58.012', '-34.345', 70000.00, 'Terreno plano');

-- 6. Contratos (10 registros)
INSERT INTO contrato (id_inquilino, id_inmueble, id_usuario_creador, fecha_desde, fecha_hasta, monto_mensual, estado, tipo) VALUES
(1, 1, 2, '2024-01-01', '2024-12-31', 80000.00, 1, 2),
(2, 2, 3, '2024-02-01', '2024-11-30', 65000.00, 1, 2),
(3, 3, 4, '2024-03-01', '2024-10-31', 95000.00, 1, 2),
(4, 4, 5, '2024-04-01', '2024-09-30', 110000.00, 1, 2),
(5, 5, 6, '2024-05-01', '2024-08-31', 50000.00, 1, 2),
(6, 6, 7, '2024-06-01', '2024-07-31', 190000.00, 1, 2),
(7, 7, 8, '2024-07-01', '2024-06-30', 70000.00, 1, 2),
(8, 8, 9, '2024-08-01', '2024-05-31', 250000.00, 1, 2),
(9, 9, 10, '2024-09-01', '2024-04-30', 35000.00, 1, 2),
(10, 10, 2, '2024-10-01', '2024-03-31', 40000.00, 1, 2);

-- 7. Imágenes (10 registros)
INSERT INTO imagen (id_inmueble, url, tipo) VALUES
(1, 'http://inmob.com/img1.jpg', 1),
(2, 'http://inmob.com/img2.jpg', 1),
(3, 'http://inmob.com/img3.jpg', 1),
(4, 'http://inmob.com/img4.jpg', 1),
(5, 'http://inmob.com/img5.jpg', 1),
(6, 'http://inmob.com/img6.jpg', 1),
(7, 'http://inmob.com/img7.jpg', 1),
(8, 'http://inmob.com/img8.jpg', 1),
(9, 'http://inmob.com/img9.jpg', 1),
(10, 'http://inmob.com/img10.jpg', 1);

-- 8. Pagos (10 registros)
INSERT INTO pago (id_contrato, id_usuario, numero_pago, fecha_pago, concepto, monto) VALUES
(1, 2, 1, '2024-01-05', 'Alquiler Enero', 80000.00),
(2, 3, 1, '2024-02-05', 'Alquiler Febrero', 65000.00),
(3, 4, 1, '2024-03-05', 'Alquiler Marzo', 95000.00),
(4, 5, 1, '2024-04-05', 'Alquiler Abril', 110000.00),
(5, 6, 1, '2024-05-05', 'Alquiler Mayo', 50000.00),
(6, 7, 1, '2024-06-05', 'Alquiler Junio', 190000.00),
(7, 8, 1, '2024-07-05', 'Alquiler Julio', 70000.00),
(8, 9, 1, '2024-08-05', 'Alquiler Agosto', 250000.00),
(9, 10, 1, '2024-09-05', 'Alquiler Septiembre', 35000.00),
(10, 2, 1, '2024-10-05', 'Alquiler Octubre', 40000.00);