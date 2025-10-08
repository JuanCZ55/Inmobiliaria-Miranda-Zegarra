-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 09-10-2025 a las 01:16:42
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria`
--
CREATE DATABASE IF NOT EXISTS `inmobiliaria` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `inmobiliaria`;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contrato`
--

CREATE TABLE `contrato` (
  `id_contrato` int(11) NOT NULL,
  `id_inquilino` int(11) NOT NULL,
  `id_inmueble` int(11) NOT NULL,
  `id_usuario_creador` int(11) DEFAULT NULL,
  `id_usuario_finalizador` int(11) DEFAULT NULL,
  `fecha_desde` date NOT NULL,
  `fecha_hasta` date NOT NULL,
  `fecha_terminacion_anticipada` date DEFAULT NULL,
  `monto_mensual` decimal(12,2) NOT NULL,
  `multa` decimal(12,2) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp(),
  `tipo` tinyint(4) NOT NULL COMMENT '1=Pago_Total, 2=Pago_Mensual'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contrato`
--

INSERT INTO `contrato` (`id_contrato`, `id_inquilino`, `id_inmueble`, `id_usuario_creador`, `id_usuario_finalizador`, `fecha_desde`, `fecha_hasta`, `fecha_terminacion_anticipada`, `monto_mensual`, `multa`, `created_at`, `updated_at`, `tipo`) VALUES
(244, 153, 169, 1, NULL, '2025-09-08', '2025-09-09', NULL, 99999.00, NULL, '2025-10-08 14:14:51', '2025-10-08 14:14:51', 1),
(246, 152, 168, 1, NULL, '2025-09-08', '2025-10-07', NULL, 9999.00, NULL, '2025-10-08 14:28:26', '2025-10-08 14:28:26', 2),
(259, 152, 168, 1, 32, '2025-10-08', '2025-10-11', '2025-10-08', 9990000.00, 0.00, '2025-10-08 14:51:51', '2025-10-08 19:59:52', 1),
(261, 152, 167, 32, NULL, '2025-09-08', '2025-10-22', NULL, 3080000.00, NULL, '2025-10-08 20:00:25', '2025-10-08 20:00:25', 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `imagen`
--

CREATE TABLE `imagen` (
  `id_imagen` int(11) NOT NULL,
  `id_inmueble` int(11) NOT NULL,
  `url` varchar(255) NOT NULL,
  `tipo` int(11) NOT NULL COMMENT '1=portada, 2=galeria'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `imagen`
--

INSERT INTO `imagen` (`id_imagen`, `id_inmueble`, `url`, `tipo`) VALUES
(271, 168, '/Uploads/Inmuebles/851eba92-1074-4685-81bf-a8c5fef2c13b.jpg', 1),
(272, 168, '/Uploads/Inmuebles/d649dfac-b422-4bca-8f4d-22b59b01c0fb.jpg', 2),
(273, 168, '/Uploads/Inmuebles/2cff1719-f47a-4b5f-bdcd-f130774c7376.jpg', 2),
(274, 168, '/Uploads/Inmuebles/f9ba58ee-1d3d-4337-8bef-0a382f76a845.jpg', 2),
(275, 168, '/Uploads/Inmuebles/3f32b8a1-d684-49cd-879b-0c632d46a4f4.jpg', 2),
(276, 168, '/Uploads/Inmuebles/9b8be873-482c-4262-9a69-f523f013d12b.jpg', 2),
(277, 168, '/Uploads/Inmuebles/20e6fd84-aba4-4eb0-ae0d-7e6983348e2b.jpg', 2),
(278, 168, '/Uploads/Inmuebles/9e8adfd4-f911-4aa9-b11e-cac48a32afe0.jpg', 2),
(279, 167, '/Uploads/Inmuebles/a7b61467-10f5-43dd-ba02-b3e1c5386c30.jpg', 1),
(280, 167, '/Uploads/Inmuebles/19709f3c-b0b2-4a4b-86a7-acedfb84db30.jpg', 2),
(281, 167, '/Uploads/Inmuebles/7758befd-ec84-47ca-88d9-f38b703761d3.jpg', 2),
(282, 167, '/Uploads/Inmuebles/eb605258-94d7-44a8-aa84-56aa3f987e70.jpg', 2),
(283, 167, '/Uploads/Inmuebles/bbfca72f-540f-4870-9dbd-68252f9df827.jpg', 2),
(284, 167, '/Uploads/Inmuebles/00aeafb6-e36b-45e4-8078-c085bb952dfd.jpg', 2),
(285, 167, '/Uploads/Inmuebles/895d5dcd-5348-40cb-b44a-fd67d6d69aca.jpg', 2),
(286, 167, '/Uploads/Inmuebles/699b4b54-fc63-48f0-9459-a979798962d2.jpg', 2),
(287, 167, '/Uploads/Inmuebles/ce561c8f-5f29-4c07-bbfc-888155daaed9.jpg', 2),
(288, 169, '/Uploads/Inmuebles/65d1796b-f3aa-4b59-8232-e20b42a305e1.jpg', 1),
(289, 169, '/Uploads/Inmuebles/8b94c29f-b13d-460d-97fc-354753d434a1.jpg', 2),
(290, 169, '/Uploads/Inmuebles/443e7604-c728-452a-8f53-7bb51b03c18a.jpg', 2),
(291, 169, '/Uploads/Inmuebles/1d80e998-bd2e-4ddd-90c5-547273993e1f.jpg', 2),
(292, 169, '/Uploads/Inmuebles/48e2c9aa-13fb-42ed-b4e1-a01a32897766.jpg', 2),
(293, 169, '/Uploads/Inmuebles/c6fb9ffc-3ffc-4407-be30-6c4274ad3917.jpg', 2);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `id_inmueble` int(11) NOT NULL,
  `id_propietario` int(11) NOT NULL,
  `id_tipo_inmueble` int(11) DEFAULT NULL,
  `direccion` varchar(255) NOT NULL,
  `uso` tinyint(4) NOT NULL COMMENT '1=residencial, 2=comercial',
  `cantidad_ambientes` int(11) NOT NULL,
  `longitud` varchar(100) NOT NULL,
  `latitud` varchar(100) NOT NULL,
  `precio` decimal(12,2) NOT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1 COMMENT '1=activo, 2=suspendido',
  `descripcion` varchar(255) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`id_inmueble`, `id_propietario`, `id_tipo_inmueble`, `direccion`, `uso`, `cantidad_ambientes`, `longitud`, `latitud`, `precio`, `estado`, `descripcion`, `created_at`, `updated_at`) VALUES
(167, 152, 2, '740, Mitre, Cuatro Avenidas, San Luis, Municipio de San Luis, Juan Martín de Pueyrredón, San Luis, D5702JRP, Argentina', 1, 4, '-66.336993', '-33.302160', 1500000.00, 1, 'Departamento céntrico y luminoso', '2025-10-07 20:04:43', '2025-10-07 20:09:16'),
(168, 151, 1, 'Viviendas Puntanas, Juana Koslay, Municipio de Juana Koslay, Juan Martín de Pueyrredón, San Luis, 5700, Argentina', 1, 8, '-66.336993', '-33.302160', 450000.00, 1, 'Casa familiar con patio grande', '2025-10-07 20:07:08', '2025-10-07 20:08:46'),
(169, 151, 1, '575, Las Heras, Cuatro Avenidas, San Luis, Municipio de San Luis, Juan Martín de Pueyrredón, San Luis, D5702JRP, Argentina', 1, 10, '-66.336993', '-33.302160', 900000.00, 2, 'Super Casa ultra lujosa demasiado mucho terreno', '2025-10-07 20:11:55', '2025-10-08 14:22:13');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `id_inquilino` int(11) NOT NULL,
  `dni` varchar(15) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `email` varchar(150) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1 COMMENT '1=activo, 2=inactivo',
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilino`
--

INSERT INTO `inquilino` (`id_inquilino`, `dni`, `nombre`, `apellido`, `telefono`, `email`, `direccion`, `estado`, `created_at`, `updated_at`) VALUES
(151, '38111222', 'Javier', 'Rodriguez', '2657334455', 'javier.r@email.com', 'Las Heras 222', 1, '2025-10-07 19:38:54', '2025-10-07 19:38:54'),
(152, '40333444', 'Maria', 'Lopez', '2657881122', 'maria.lopez@email.com', 'Pedernera 555', 1, '2025-10-07 19:39:28', '2025-10-07 19:39:28'),
(153, '36555666', 'Roberto', 'Martinez', '2657667788', 'roberto.m@email.com', 'Chacabuco 888', 1, '2025-10-07 19:40:01', '2025-10-07 19:40:01');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `id_pago` int(11) NOT NULL,
  `id_contrato` int(11) NOT NULL,
  `id_usuario` int(11) DEFAULT NULL,
  `numero_pago` smallint(6) NOT NULL,
  `fecha_pago` date NOT NULL,
  `concepto` varchar(255) NOT NULL,
  `monto` decimal(12,2) NOT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1 COMMENT '1=valido, 2=anulado',
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pago`
--

INSERT INTO `pago` (`id_pago`, `id_contrato`, `id_usuario`, `numero_pago`, `fecha_pago`, `concepto`, `monto`, `estado`, `created_at`, `updated_at`) VALUES
(68, 244, 1, 1, '2025-10-08', 'Deposito de todo el Alquiler del Contrato', 99999.00, 1, '2025-10-08 14:14:51', '2025-10-08 14:14:51'),
(69, 246, 1, 1, '2025-10-08', 'Deposito de un mes', 9999.00, 1, '2025-10-08 14:28:26', '2025-10-08 14:28:26'),
(70, 259, 1, 1, '2025-10-08', 'Deposito de todo el Alquiler del Contrato', 999900.00, 1, '2025-10-08 14:51:51', '2025-10-08 14:51:51'),
(72, 259, NULL, 2, '2025-10-08', 'Rompio un visagra', 90000.00, 1, '2025-10-08 19:55:02', '2025-10-08 19:55:02'),
(73, 259, 32, 3, '2025-10-08', 'Multa de Cancelacion', 0.00, 1, '2025-10-08 19:59:52', '2025-10-08 19:59:52'),
(74, 261, 32, 1, '2025-10-08', 'Deposito de todo el Alquiler del Contrato', 3080000.00, 1, '2025-10-08 20:00:25', '2025-10-08 20:00:25');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `id_propietario` int(11) NOT NULL,
  `dni` varchar(15) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `email` varchar(150) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1 COMMENT '1=activo, 2=inactivo',
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`id_propietario`, `dni`, `nombre`, `apellido`, `telefono`, `email`, `direccion`, `estado`, `created_at`, `updated_at`) VALUES
(151, '25111222', 'Carlos', 'Gomez', '2664558877', 'carlos.gomez@email.com', 'Av. Illia 123', 1, '2025-10-07 18:34:24', '2025-10-07 18:38:02'),
(152, '30333444', 'Susana', 'Perez', '2664990011', 'susana.perez@email.com', 'Rivadavia 456', 1, '2025-10-07 18:39:28', '2025-10-07 18:39:28');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo_inmueble`
--

CREATE TABLE `tipo_inmueble` (
  `id_tipo_inmueble` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipo_inmueble`
--

INSERT INTO `tipo_inmueble` (`id_tipo_inmueble`, `nombre`, `created_at`, `updated_at`) VALUES
(1, 'Casa', '2025-09-21 21:03:06', '2025-09-21 21:03:06'),
(2, 'Departamento', '2025-09-21 21:03:06', '2025-09-21 21:03:06'),
(3, 'Local', '2025-09-21 21:03:06', '2025-09-21 21:03:06'),
(4, 'Oficina', '2025-09-21 21:03:06', '2025-09-21 21:03:06'),
(5, 'Galpon', '2025-09-21 21:03:06', '2025-09-21 21:03:06'),
(6, 'Hotel', '2025-09-21 21:03:06', '2025-09-21 21:03:06'),
(7, 'Cabaña', '2025-09-21 21:03:06', '2025-09-21 21:03:06'),
(8, 'Edificio', '2025-09-21 21:03:06', '2025-09-21 21:03:06'),
(9, 'Cochera', '2025-09-21 21:03:06', '2025-09-21 21:03:06'),
(10, 'Terreno', '2025-09-21 21:03:06', '2025-09-21 21:03:06');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `id_usuario` int(11) NOT NULL,
  `email` varchar(150) NOT NULL,
  `contrasena` varchar(255) NOT NULL,
  `rol` tinyint(4) NOT NULL COMMENT '1=administrador, 2=empleado',
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `url` varchar(255) DEFAULT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1 COMMENT '1=activo, 2=inactivo',
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp(),
  `genero` int(11) DEFAULT NULL COMMENT '1=masculino, 2 femenino'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`id_usuario`, `email`, `contrasena`, `rol`, `nombre`, `apellido`, `url`, `estado`, `created_at`, `updated_at`, `genero`) VALUES
(1, 'user01@gmail.com', 'AQAAAAIAAYagAAAAEJe/0FDlu1A56hCXRObw/IQIVvBwuRHpO/rY+VOxw8ovSuBcExD08VeAUC/FVGygHw==', 1, 'El Hechizero', 'El Gran Hechizero', NULL, 1, '2025-10-02 14:49:00', '2025-10-08 20:11:39', 1),
(13, 'user02@gmail.com', 'AQAAAAIAAYagAAAAEKg5Vve4ODlWkm+bML/CpVDBgCCwaE0JRfUgu5GKAVMSLpzMzJkSxf4UAEBnZJxRTw==', 2, 'Juan', 'Garcia', NULL, 1, '2025-10-04 01:31:48', '2025-10-04 01:31:48', 1),
(32, 'user03@gmail.com', 'AQAAAAIAAYagAAAAEGIjDl2Ix31csvoXEp8BaHGVkKXN8yexfuWxGPrSRTvzJ3a7W8JgLN0ADuRYswGZbA==', 2, 'Juan', 'Figueroa', NULL, 2, '2025-10-08 19:59:10', '2025-10-08 20:02:16', 1),
(33, 'user04@gmail.com', 'AQAAAAIAAYagAAAAEC6A03OZhCFYwdO8y0V9Nit5VQhJfwAt1vPZdjAyBdCvc3/S0a60E4cX5OB2ivZLjQ==', 1, 'Filipa', 'Dominguez', NULL, 1, '2025-10-08 20:08:52', '2025-10-08 20:08:52', 2);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`id_contrato`),
  ADD KEY `idx_contrato_inmueble` (`id_inmueble`),
  ADD KEY `idx_contrato_inquilino` (`id_inquilino`),
  ADD KEY `idx_contrato_vigencia` (`fecha_desde`,`fecha_hasta`),
  ADD KEY `idx_contrato_tipo` (`tipo`),
  ADD KEY `id_usuario_creador` (`id_usuario_creador`),
  ADD KEY `id_usuario_finalizador` (`id_usuario_finalizador`);

--
-- Indices de la tabla `imagen`
--
ALTER TABLE `imagen`
  ADD PRIMARY KEY (`id_imagen`),
  ADD UNIQUE KEY `uk_imagen_url` (`url`),
  ADD KEY `idx_imagen_inmueble` (`id_inmueble`);

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`id_inmueble`),
  ADD KEY `idx_inmueble_estado` (`estado`),
  ADD KEY `idx_inmueble_propietario` (`id_propietario`),
  ADD KEY `idx_inmueble_tipo` (`id_tipo_inmueble`),
  ADD KEY `idx_inmueble_direccion` (`direccion`),
  ADD KEY `idx_inmueble_precio` (`precio`),
  ADD KEY `idx_inmueble_uso` (`uso`),
  ADD KEY `idx_inmueble_ambientes` (`cantidad_ambientes`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`id_inquilino`),
  ADD UNIQUE KEY `uk_inquilino_dni` (`dni`),
  ADD KEY `idx_inquilino_nombre` (`nombre`),
  ADD KEY `idx_inquilino_apellido` (`apellido`),
  ADD KEY `idx_inquilino_estado` (`estado`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`id_pago`),
  ADD KEY `idx_pago_contrato` (`id_contrato`),
  ADD KEY `idx_pago_usuario` (`id_usuario`),
  ADD KEY `idx_pago_estado` (`estado`),
  ADD KEY `idx_pago_fecha` (`fecha_pago`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`id_propietario`),
  ADD UNIQUE KEY `uk_propietario_dni` (`dni`),
  ADD KEY `idx_propietario_nombre` (`nombre`),
  ADD KEY `idx_propietario_apellido` (`apellido`),
  ADD KEY `idx_propietario_estado` (`estado`);

--
-- Indices de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  ADD PRIMARY KEY (`id_tipo_inmueble`),
  ADD UNIQUE KEY `uk_tipo_inmueble_nombre` (`nombre`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id_usuario`),
  ADD UNIQUE KEY `uk_usuario_email` (`email`),
  ADD KEY `idx_usuario_nombre` (`nombre`),
  ADD KEY `idx_usuario_apellido` (`apellido`),
  ADD KEY `idx_usuario_estado` (`estado`),
  ADD KEY `idx_usuario_rol` (`rol`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contrato`
--
ALTER TABLE `contrato`
  MODIFY `id_contrato` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=262;

--
-- AUTO_INCREMENT de la tabla `imagen`
--
ALTER TABLE `imagen`
  MODIFY `id_imagen` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=294;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id_inmueble` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=170;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id_inquilino` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=154;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `id_pago` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=75;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id_propietario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=153;

--
-- AUTO_INCREMENT de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  MODIFY `id_tipo_inmueble` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id_usuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=34;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilino` (`id_inquilino`) ON UPDATE CASCADE,
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id_inmueble`) ON UPDATE CASCADE,
  ADD CONSTRAINT `contrato_ibfk_3` FOREIGN KEY (`id_usuario_creador`) REFERENCES `usuario` (`id_usuario`) ON UPDATE CASCADE,
  ADD CONSTRAINT `contrato_ibfk_4` FOREIGN KEY (`id_usuario_finalizador`) REFERENCES `usuario` (`id_usuario`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Filtros para la tabla `imagen`
--
ALTER TABLE `imagen`
  ADD CONSTRAINT `imagen_ibfk_1` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id_inmueble`) ON DELETE CASCADE;

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id_propietario`) ON UPDATE CASCADE,
  ADD CONSTRAINT `inmueble_ibfk_2` FOREIGN KEY (`id_tipo_inmueble`) REFERENCES `tipo_inmueble` (`id_tipo_inmueble`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id_contrato`) ON UPDATE CASCADE,
  ADD CONSTRAINT `pago_ibfk_2` FOREIGN KEY (`id_usuario`) REFERENCES `usuario` (`id_usuario`) ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
