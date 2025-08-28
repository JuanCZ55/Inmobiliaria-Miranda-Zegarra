CREATE DATABASE inmobiliaria;
USE inmobiliaria;

CREATE TABLE `usuario` (
  `id_usuario` INTEGER PRIMARY KEY AUTO_INCREMENT,
  `email` VARCHAR(150) UNIQUE NOT NULL,
  `contrasena` VARCHAR(255) NOT NULL,
  `rol` TINYINT NOT NULL COMMENT '1=administrador, 2=empleado',
  `nombre` VARCHAR(100) NOT NULL,
  `apellido` VARCHAR(100) NOT NULL,
  `avatar` MEDIUMBLOB,
  `estado` TINYINT NOT NULL DEFAULT 1 COMMENT '1=activo, 2=inactivo',
  `created_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  `updated_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE `propietario` (
  `id_propietario` INTEGER PRIMARY KEY AUTO_INCREMENT,
  `dni` VARCHAR(15) UNIQUE NOT NULL,
  `nombre` VARCHAR(100) NOT NULL,
  `apellido` VARCHAR(100) NOT NULL,
  `telefono` VARCHAR(20) NOT NULL,
  `email` VARCHAR(150) NOT NULL,
  `direccion` VARCHAR(255) NOT NULL,
  `estado` TINYINT NOT NULL DEFAULT 1 COMMENT '1=activo, 2=inactivo',
  `created_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  `updated_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE `tipo_inmueble` (
  `id_tipo_inmueble` INTEGER PRIMARY KEY AUTO_INCREMENT,
  `nombre` VARCHAR(50) UNIQUE NOT NULL,
  `created_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  `updated_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE `inmueble` (
  `id_inmueble` INTEGER PRIMARY KEY AUTO_INCREMENT,
  `id_propietario` INTEGER NOT NULL,
  `id_tipo_inmueble` INTEGER NULL,
  `direccion` VARCHAR(255) NOT NULL,
  `uso` TINYINT NOT NULL COMMENT '1=residencial, 2=comercial',
  `cantidad_ambientes` INTEGER NOT NULL,
  `longitud` VARCHAR(100) NOT NULL,
  `latitud` VARCHAR(100) NOT NULL,
  `precio` DECIMAL(12,2) NOT NULL,
  `estado` TINYINT NOT NULL DEFAULT 1 COMMENT '1=disponible, 2=ocupado, 3=suspendido',
  `descripcion` VARCHAR(255),
  `created_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  `updated_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE `inquilino` (
  `id_inquilino` INTEGER PRIMARY KEY AUTO_INCREMENT,
  `dni` VARCHAR(15) UNIQUE NOT NULL,
  `nombre` VARCHAR(100) NOT NULL,
  `apellido` VARCHAR(100) NOT NULL,
  `telefono` VARCHAR(20) NOT NULL,
  `email` VARCHAR(150) NOT NULL,
  `direccion` VARCHAR(255) NOT NULL,
  `estado` TINYINT NOT NULL DEFAULT 1 COMMENT '1=activo, 2=inactivo',
  `created_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  `updated_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE `contrato` (
  `id_contrato` INTEGER PRIMARY KEY AUTO_INCREMENT,
  `id_inquilino` INTEGER NOT NULL,
  `id_inmueble` INTEGER NOT NULL,
  `id_usuario_creador` INTEGER NOT NULL,
  `id_usuario_finalizador` INTEGER,
  `fecha_desde` DATE NOT NULL,
  `fecha_hasta` DATE NOT NULL,
  `fecha_terminacion_anticipada` DATE,
  `monto_mensual` DECIMAL(12,2) NOT NULL,
  `multa` DECIMAL(12,2),
  `estado` TINYINT NOT NULL COMMENT '1=vigente,2=finalizado,3=cancelado',
  `created_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  `updated_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

CREATE TABLE `pago` (
  `id_pago` INTEGER PRIMARY KEY AUTO_INCREMENT,
  `id_contrato` INTEGER NOT NULL,
  `id_usuario` INTEGER NOT NULL,
  `numero_pago` SMALLINT NOT NULL,
  `fecha_pago` DATE NOT NULL,
  `concepto` VARCHAR(255) NOT NULL,
  `monto` DECIMAL(12,2) NOT NULL,
  `estado` TINYINT NOT NULL DEFAULT 1 COMMENT '1=valido, 2=anulado',
  `created_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP),
  `updated_at` DATETIME NOT NULL DEFAULT (CURRENT_TIMESTAMP)
);

-- indices
CREATE INDEX `idx_inmueble_estado` ON `inmueble` (`estado`);
CREATE INDEX `idx_inmueble_propietario` ON `inmueble` (`id_propietario`);
CREATE INDEX `idx_contrato_inmueble` ON `contrato` (`id_inmueble`);
CREATE INDEX `idx_contrato_inquilino` ON `contrato` (`id_inquilino`);
CREATE INDEX `idx_contrato_vigencia` ON `contrato` (`fecha_desde`, `fecha_hasta`);
CREATE INDEX `idx_pago_contrato` ON `pago` (`id_contrato`);

-- relaciones
ALTER TABLE `inmueble` 
  ADD FOREIGN KEY (`id_propietario`) 
  REFERENCES `propietario` (`id_propietario`) 
  ON DELETE NO ACTION ON UPDATE CASCADE;

ALTER TABLE `inmueble` 
  ADD FOREIGN KEY (`id_tipo_inmueble`) 
  REFERENCES `tipo_inmueble` (`id_tipo_inmueble`) 
  ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE `contrato` 
  ADD FOREIGN KEY (`id_inquilino`) 
  REFERENCES `inquilino` (`id_inquilino`) 
  ON DELETE NO ACTION ON UPDATE CASCADE;

ALTER TABLE `contrato` 
  ADD FOREIGN KEY (`id_inmueble`) 
  REFERENCES `inmueble` (`id_inmueble`) 
  ON DELETE NO ACTION ON UPDATE CASCADE;

ALTER TABLE `contrato` 
  ADD FOREIGN KEY (`id_usuario_creador`) 
  REFERENCES `usuario` (`id_usuario`) 
  ON DELETE NO ACTION ON UPDATE CASCADE;

ALTER TABLE `contrato` 
  ADD FOREIGN KEY (`id_usuario_finalizador`) 
  REFERENCES `usuario` (`id_usuario`) 
  ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE `pago` 
  ADD FOREIGN KEY (`id_contrato`) 
  REFERENCES `contrato` (`id_contrato`) 
  ON DELETE NO ACTION ON UPDATE CASCADE;

ALTER TABLE `pago` 
  ADD FOREIGN KEY (`id_usuario`) 
  REFERENCES `usuario` (`id_usuario`) 
  ON DELETE NO ACTION ON UPDATE CASCADE;
