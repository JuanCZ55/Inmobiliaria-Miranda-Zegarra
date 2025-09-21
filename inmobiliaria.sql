-- Crear la base de datos 
CREATE DATABASE IF NOT EXISTS `inmobiliaria` 
  DEFAULT CHARACTER SET utf8mb4 
  COLLATE utf8mb4_0900_ai_ci;
USE `inmobiliaria`;

-- ==========================
-- Tablas base (sin dependencias)
-- ==========================
CREATE TABLE `usuario` (
  `id_usuario` int NOT NULL AUTO_INCREMENT,
  `email` varchar(150) NOT NULL,
  `contrasena` varchar(255) NOT NULL,
  `rol` tinyint NOT NULL COMMENT '1=administrador, 2=empleado',
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `url` varchar(255),
  `estado` tinyint NOT NULL DEFAULT '1' COMMENT '1=activo, 2=inactivo',
  `created_at` datetime NOT NULL DEFAULT (now()),
  `updated_at` datetime NOT NULL DEFAULT (now()),
  PRIMARY KEY (`id_usuario`),
  UNIQUE KEY `uk_usuario_email` (`email`),
  KEY `idx_usuario_nombre` (`nombre`),
  KEY `idx_usuario_apellido` (`apellido`),
  KEY `idx_usuario_estado` (`estado`),
  KEY `idx_usuario_rol` (`rol`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `inquilino` (
  `id_inquilino` int NOT NULL AUTO_INCREMENT,
  `dni` varchar(15) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `email` varchar(150) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `estado` tinyint NOT NULL DEFAULT '1' COMMENT '1=activo, 2=inactivo',
  `created_at` datetime NOT NULL DEFAULT (now()),
  `updated_at` datetime NOT NULL DEFAULT (now()),
  PRIMARY KEY (`id_inquilino`),
  UNIQUE KEY `uk_inquilino_dni` (`dni`),
  KEY `idx_inquilino_nombre` (`nombre`),
  KEY `idx_inquilino_apellido` (`apellido`),
  KEY `idx_inquilino_estado` (`estado`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `propietario` (
  `id_propietario` int NOT NULL AUTO_INCREMENT,
  `dni` varchar(15) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `telefono` varchar(20) NOT NULL,
  `email` varchar(150) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `estado` tinyint NOT NULL DEFAULT '1' COMMENT '1=activo, 2=inactivo',
  `created_at` datetime NOT NULL DEFAULT (now()),
  `updated_at` datetime NOT NULL DEFAULT (now()),
  PRIMARY KEY (`id_propietario`),
  UNIQUE KEY `uk_propietario_dni` (`dni`),
  KEY `idx_propietario_nombre` (`nombre`),
  KEY `idx_propietario_apellido` (`apellido`),
  KEY `idx_propietario_estado` (`estado`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `tipo_inmueble` (
  `id_tipo_inmueble` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT (now()),
  `updated_at` datetime NOT NULL DEFAULT (now()),
  PRIMARY KEY (`id_tipo_inmueble`),
  UNIQUE KEY `uk_tipo_inmueble_nombre` (`nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- ==========================
-- Tablas que dependen de las anteriores
-- ==========================
CREATE TABLE `inmueble` (
  `id_inmueble` int NOT NULL AUTO_INCREMENT,
  `id_propietario` int NOT NULL,
  `id_tipo_inmueble` int DEFAULT NULL,
  `direccion` varchar(255) NOT NULL,
  `uso` tinyint NOT NULL COMMENT '1=residencial, 2=comercial',
  `cantidad_ambientes` int NOT NULL,
  `longitud` varchar(100) NOT NULL,
  `latitud` varchar(100) NOT NULL,
  `precio` decimal(12,2) NOT NULL,
  `estado` tinyint NOT NULL DEFAULT '1' COMMENT '1=disponible, 2=suspendido',
  `descripcion` varchar(255) DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT (now()),
  `updated_at` datetime NOT NULL DEFAULT (now()),
  PRIMARY KEY (`id_inmueble`),
  KEY `idx_inmueble_estado` (`estado`),
  KEY `idx_inmueble_propietario` (`id_propietario`),
  KEY `idx_inmueble_tipo` (`id_tipo_inmueble`),
  KEY `idx_inmueble_direccion` (`direccion`),
  KEY `idx_inmueble_precio` (`precio`),
  KEY `idx_inmueble_uso` (`uso`),
  KEY `idx_inmueble_ambientes` (`cantidad_ambientes`),
  CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`id_propietario`) REFERENCES `propietario` (`id_propietario`) ON UPDATE CASCADE,
  CONSTRAINT `inmueble_ibfk_2` FOREIGN KEY (`id_tipo_inmueble`) REFERENCES `tipo_inmueble` (`id_tipo_inmueble`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `contrato` (
  `id_contrato` int NOT NULL AUTO_INCREMENT,
  `id_inquilino` int NOT NULL,
  `id_inmueble` int NOT NULL,
  `id_usuario_creador` int DEFAULT NULL,
  `id_usuario_finalizador` int DEFAULT NULL,
  `fecha_desde` date NOT NULL,
  `fecha_hasta` date NOT NULL,
  `fecha_terminacion_anticipada` date DEFAULT NULL,
  `monto_mensual` decimal(12,2) NOT NULL,
  `multa` decimal(12,2) DEFAULT NULL,
  `estado` tinyint NOT NULL COMMENT '1=vigente,2=finalizado,3=cancelado',
  `created_at` datetime NOT NULL DEFAULT (now()),
  `updated_at` datetime NOT NULL DEFAULT (now()),
  `tipo` tinyint NOT NULL COMMENT '1=Pago_Total, 2=Pago_Mensual',
  PRIMARY KEY (`id_contrato`),
  KEY `idx_contrato_inmueble` (`id_inmueble`),
  KEY `idx_contrato_inquilino` (`id_inquilino`),
  KEY `idx_contrato_estado` (`estado`),
  KEY `idx_contrato_vigencia` (`fecha_desde`,`fecha_hasta`),
  KEY `idx_contrato_tipo` (`tipo`),
  KEY `id_usuario_creador` (`id_usuario_creador`),
  KEY `id_usuario_finalizador` (`id_usuario_finalizador`),
  CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`id_inquilino`) REFERENCES `inquilino` (`id_inquilino`) ON UPDATE CASCADE,
  CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id_inmueble`) ON UPDATE CASCADE,
  CONSTRAINT `contrato_ibfk_3` FOREIGN KEY (`id_usuario_creador`) REFERENCES `usuario` (`id_usuario`) ON UPDATE CASCADE,
  CONSTRAINT `contrato_ibfk_4` FOREIGN KEY (`id_usuario_finalizador`) REFERENCES `usuario` (`id_usuario`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `imagen` (
  `id_imagen` int NOT NULL AUTO_INCREMENT,
  `id_inmueble` int NOT NULL,
  `url` varchar(255) NOT NULL,
  `tipo` int NOT NULL COMMENT '1=portada, 2=galeria',
  PRIMARY KEY (`id_imagen`),
  UNIQUE KEY `uk_imagen_url` (`url`),
  KEY `idx_imagen_inmueble` (`id_inmueble`),
  CONSTRAINT `imagen_ibfk_1` FOREIGN KEY (`id_inmueble`) REFERENCES `inmueble` (`id_inmueble`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `pago` (
  `id_pago` int NOT NULL AUTO_INCREMENT,
  `id_contrato` int NOT NULL,
  `id_usuario` int DEFAULT NULL,
  `numero_pago` smallint NOT NULL,
  `fecha_pago` date NOT NULL,
  `concepto` varchar(255) NOT NULL,
  `monto` decimal(12,2) NOT NULL,
  `estado` tinyint NOT NULL DEFAULT '1' COMMENT '1=valido, 2=anulado',
  `created_at` datetime NOT NULL DEFAULT (now()),
  `updated_at` datetime NOT NULL DEFAULT (now()),
  PRIMARY KEY (`id_pago`),
  KEY `idx_pago_contrato` (`id_contrato`),
  KEY `idx_pago_usuario` (`id_usuario`),
  KEY `idx_pago_estado` (`estado`),
  KEY `idx_pago_fecha` (`fecha_pago`),
  CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id_contrato`) ON UPDATE CASCADE,
  CONSTRAINT `pago_ibfk_2` FOREIGN KEY (`id_usuario`) REFERENCES `usuario` (`id_usuario`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
