-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 22-08-2025 a las 22:49:39
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
  `id_usuario_creador` int(11) NOT NULL,
  `id_usuario_finalizador` int(11) DEFAULT NULL,
  `fecha_desde` date NOT NULL,
  `fecha_hasta` date NOT NULL,
  `fecha_terminacion_anticipada` date DEFAULT NULL,
  `monto_mensual` decimal(12,2) NOT NULL,
  `multa` decimal(12,2) DEFAULT NULL,
  `observaciones` varchar(255) DEFAULT NULL,
  `estado` tinyint(4) NOT NULL COMMENT '1=vigente,2=finalizado,3=cancelado,4=suspendido',
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `id_inmueble` int(11) NOT NULL,
  `id_propietario` int(11) NOT NULL,
  `id_tipo_inmueble` int(11) NOT NULL DEFAULT 0,
  `direccion` varchar(255) NOT NULL,
  `uso` tinyint(4) NOT NULL COMMENT '1=residencial, 2=comercial',
  `cantidad_ambientes` int(11) NOT NULL,
  `coordenadas` varchar(100) NOT NULL,
  `precio` decimal(12,2) NOT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1 COMMENT '1=disponible, 2=ocupado, 3=suspendido',
  `descripcion` varchar(255) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `id_pago` int(11) NOT NULL,
  `id_contrato` int(11) NOT NULL,
  `id_usuario` int(11) NOT NULL,
  `numero_pago` smallint(6) NOT NULL,
  `fecha_pago` date NOT NULL,
  `concepto` varchar(255) NOT NULL,
  `monto` decimal(12,2) NOT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1 COMMENT '1=valido, 2=anulado',
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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
  `avatar` varchar(255) DEFAULT NULL,
  `estado` tinyint(4) NOT NULL DEFAULT 1 COMMENT '1=activo, 2=inactivo',
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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
  ADD KEY `id_usuario_creador` (`id_usuario_creador`),
  ADD KEY `id_usuario_finalizador` (`id_usuario_finalizador`);

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`id_inmueble`),
  ADD KEY `idx_inmueble_estado` (`estado`),
  ADD KEY `idx_inmueble_propietario` (`id_propietario`),
  ADD KEY `id_tipo_inmueble` (`id_tipo_inmueble`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`id_inquilino`),
  ADD UNIQUE KEY `dni` (`dni`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`id_pago`),
  ADD KEY `idx_pago_contrato` (`id_contrato`),
  ADD KEY `id_usuario` (`id_usuario`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`id_propietario`),
  ADD UNIQUE KEY `dni` (`dni`);

--
-- Indices de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  ADD PRIMARY KEY (`id_tipo_inmueble`),
  ADD UNIQUE KEY `nombre` (`nombre`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id_usuario`),
  ADD UNIQUE KEY `email` (`email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `contrato`
--
ALTER TABLE `contrato`
  MODIFY `id_contrato` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id_inmueble` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id_inquilino` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `id_pago` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id_propietario` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `tipo_inmueble`
--
ALTER TABLE `tipo_inmueble`
  MODIFY `id_tipo_inmueble` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id_usuario` int(11) NOT NULL AUTO_INCREMENT;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilino` (`id_inquilino`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id_inmueble`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `contrato_ibfk_3` FOREIGN KEY (`id_usuario_creador`) REFERENCES `usuario` (`id_usuario`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `contrato_ibfk_4` FOREIGN KEY (`id_usuario_finalizador`) REFERENCES `usuario` (`id_usuario`) ON DELETE SET NULL ON UPDATE CASCADE;

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id_propietario`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `inmueble_ibfk_2` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id_propietario`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `inmueble_ibfk_3` FOREIGN KEY (`id_tipo_inmueble`) REFERENCES `tipo_inmueble` (`id_tipo_inmueble`) ON UPDATE CASCADE;

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id_contrato`) ON DELETE NO ACTION ON UPDATE CASCADE,
  ADD CONSTRAINT `pago_ibfk_2` FOREIGN KEY (`id_usuario`) REFERENCES `usuario` (`id_usuario`) ON DELETE NO ACTION ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
