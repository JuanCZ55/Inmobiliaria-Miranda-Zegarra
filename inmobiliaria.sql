CREATE TABLE `usuario` (
	`id_usuario` INTEGER AUTO_INCREMENT,
	`email` VARCHAR(150) NOT NULL UNIQUE,
	`contrasena` VARCHAR(255) NOT NULL,
	`rol` TINYINT NOT NULL COMMENT '1=administrador, 2=empleado',
	`nombre` VARCHAR(100) NOT NULL,
	`apellido` VARCHAR(100) NOT NULL,
	`avatar` MEDIUMBLOB,
	`estado` TINYINT NOT NULL DEFAULT 1 COMMENT '1=activo, 2=inactivo',
	`created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY(`id_usuario`)
);


CREATE TABLE `propietario` (
	`id_propietario` INTEGER AUTO_INCREMENT,
	`dni` VARCHAR(15) NOT NULL UNIQUE,
	`nombre` VARCHAR(100) NOT NULL,
	`apellido` VARCHAR(100) NOT NULL,
	`telefono` VARCHAR(20) NOT NULL,
	`email` VARCHAR(150) NOT NULL,
	`direccion` VARCHAR(255) NOT NULL,
	`estado` TINYINT NOT NULL DEFAULT 1 COMMENT '1=activo, 2=inactivo',
	`created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY(`id_propietario`)
);


CREATE TABLE `tipo_inmueble` (
	`id_tipo_inmueble` INTEGER NOT NULL AUTO_INCREMENT,
	`nombre` VARCHAR(50) NOT NULL UNIQUE,
	`created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY(`id_tipo_inmueble`)
);


CREATE TABLE `inmueble` (
	`id_inmueble` INTEGER NOT NULL AUTO_INCREMENT,
	`id_propietario` INTEGER NOT NULL,
	`id_tipo_inmueble` INTEGER NOT NULL DEFAULT 0,
	`direccion` VARCHAR(255) NOT NULL,
	`uso` TINYINT NOT NULL COMMENT '1=residencial, 2=comercial',
	`cantidad_ambientes` INTEGER NOT NULL,
	`longitud` VARCHAR(100) NOT NULL,
	`latitud` VARCHAR(100) NOT NULL,
	`precio` DECIMAL(12,2) NOT NULL,
	`estado` TINYINT NOT NULL DEFAULT 1 COMMENT '1=disponible, 2=ocupado, 3=suspendido',
	`descripcion` VARCHAR(255),
	`created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY(`id_inmueble`)
);


CREATE INDEX `idx_inmueble_estado`
ON `inmueble` (`estado`);
CREATE INDEX `idx_inmueble_propietario`
ON `inmueble` (`id_propietario`);
CREATE TABLE `inquilino` (
	`id_inquilino` INTEGER NOT NULL AUTO_INCREMENT,
	`dni` VARCHAR(15) NOT NULL UNIQUE,
	`nombre` VARCHAR(100) NOT NULL,
	`apellido` VARCHAR(100) NOT NULL,
	`telefono` VARCHAR(20) NOT NULL,
	`email` VARCHAR(150) NOT NULL,
	`direccion` VARCHAR(255) NOT NULL,
	`estado` TINYINT NOT NULL DEFAULT 1 COMMENT '1=activo, 2=inactivo',
	`created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY(`id_inquilino`)
);


CREATE TABLE `contrato` (
	`id_contrato` INTEGER NOT NULL AUTO_INCREMENT,
	`id_inquilino` INTEGER NOT NULL,
	`id_inmueble` INTEGER NOT NULL,
	`id_usuario_creador` INTEGER NOT NULL,
	`id_usuario_finalizador` INTEGER,
	`fecha_desde` DATE NOT NULL,
	`fecha_hasta` DATE NOT NULL,
	`fecha_terminacion_anticipada` DATE,
	`monto_mensual` DECIMAL(12,2) NOT NULL,
	`multa` DECIMAL(12,2),
	`observaciones` VARCHAR(255),
	`estado` TINYINT NOT NULL COMMENT '1=vigente,2=finalizado,3=cancelado,4=suspendido',
	`created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY(`id_contrato`)
);


CREATE INDEX `idx_contrato_inmueble`
ON `contrato` (`id_inmueble`);
CREATE INDEX `idx_contrato_inquilino`
ON `contrato` (`id_inquilino`);
CREATE INDEX `idx_contrato_vigencia`
ON `contrato` (`fecha_desde`, `fecha_hasta`);
CREATE TABLE `pago` (
	`id_pago` INTEGER NOT NULL AUTO_INCREMENT,
	`id_contrato` INTEGER NOT NULL,
	`id_usuario` INTEGER NOT NULL,
	`numero_pago` SMALLINT NOT NULL,
	`fecha_pago` DATE NOT NULL,
	`concepto` VARCHAR(255) NOT NULL,
	`monto` DECIMAL(12,2) NOT NULL,
	`estado` TINYINT NOT NULL DEFAULT 1 COMMENT '1=valido, 2=anulado',
	`created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`updated_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY(`id_pago`)
);


CREATE INDEX `idx_pago_contrato`
ON `pago` (`id_contrato`);
ALTER TABLE `inmueble`
ADD FOREIGN KEY(`id_propietario`) REFERENCES `propietario`(`id_propietario`)
ON UPDATE CASCADE ON DELETE NO ACTION;
ALTER TABLE `inmueble`
ADD FOREIGN KEY(`id_tipo_inmueble`) REFERENCES `tipo_inmueble`(`id_tipo_inmueble`)
ON UPDATE CASCADE ON DELETE SET DEFAULT;
ALTER TABLE `contrato`
ADD FOREIGN KEY(`id_inquilino`) REFERENCES `inquilino`(`id_inquilino`)
ON UPDATE CASCADE ON DELETE NO ACTION;
ALTER TABLE `contrato`
ADD FOREIGN KEY(`id_inmueble`) REFERENCES `inmueble`(`id_inmueble`)
ON UPDATE CASCADE ON DELETE NO ACTION;
ALTER TABLE `contrato`
ADD FOREIGN KEY(`id_usuario_creador`) REFERENCES `usuario`(`id_usuario`)
ON UPDATE CASCADE ON DELETE NO ACTION;
ALTER TABLE `contrato`
ADD FOREIGN KEY(`id_usuario_finalizador`) REFERENCES `usuario`(`id_usuario`)
ON UPDATE CASCADE ON DELETE SET NULL;
ALTER TABLE `pago`
ADD FOREIGN KEY(`id_contrato`) REFERENCES `contrato`(`id_contrato`)
ON UPDATE CASCADE ON DELETE NO ACTION;
ALTER TABLE `pago`
ADD FOREIGN KEY(`id_usuario`) REFERENCES `usuario`(`id_usuario`)
ON UPDATE CASCADE ON DELETE NO ACTION;