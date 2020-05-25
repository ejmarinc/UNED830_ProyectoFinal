-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema TransportesCRPF
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `TransportesCRPF` ;

-- -----------------------------------------------------
-- Schema TransportesCRPF
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `TransportesCRPF` DEFAULT CHARACTER SET utf8 ;
USE `TransportesCRPF` ;

-- -----------------------------------------------------
-- Table `TransportesCRPF`.`Conductor`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `TransportesCRPF`.`Conductor` ;

CREATE TABLE IF NOT EXISTS `TransportesCRPF`.`Conductor` (
  `Identificacion` VARCHAR(20) NOT NULL,
  `Nombre` VARCHAR(45) NULL,
  `PrimerApellido` VARCHAR(45) NULL,
  `SegundoApellido` VARCHAR(45) NULL,
  `Usuario` VARCHAR(45) NULL,
  `Contrasena` VARCHAR(45) NULL,
  `Validado` VARCHAR(10) NULL,
  PRIMARY KEY (`Identificacion`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `TransportesCRPF`.`Camion`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `TransportesCRPF`.`Camion` ;

CREATE TABLE IF NOT EXISTS `TransportesCRPF`.`Camion` (
  `Placa` VARCHAR(10) NOT NULL,
  `Marca` VARCHAR(45) NULL,
  `Modelo` VARCHAR(45) NULL,
  PRIMARY KEY (`Placa`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `TransportesCRPF`.`ConductorCamion`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `TransportesCRPF`.`ConductorCamion` ;

CREATE TABLE IF NOT EXISTS `TransportesCRPF`.`ConductorCamion` (
  `Identificacion` VARCHAR(20) NOT NULL,
  `Placa` VARCHAR(10) NOT NULL,
  PRIMARY KEY (`Identificacion`, `Placa`),
  CONSTRAINT `Identificacion`
    FOREIGN KEY ()
    REFERENCES `TransportesCRPF`.`Conductor` ()
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `Placa`
    FOREIGN KEY ()
    REFERENCES `TransportesCRPF`.`Camion` ()
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `TransportesCRPF`.`ConductorViaje`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `TransportesCRPF`.`ConductorViaje` ;

CREATE TABLE IF NOT EXISTS `TransportesCRPF`.`ConductorViaje` (
  `Identificacion` VARCHAR(20) NOT NULL,
  `LugarInicio` VARCHAR(45) NOT NULL,
  `LugarFinalizacion` VARCHAR(45) NOT NULL,
  `Carga` VARCHAR(45) NOT NULL,
  `TiempoEstimado` VARCHAR(20) NOT NULL,
  `ConductorViajecol` VARCHAR(45) NOT NULL,
  `TrackingID` INT NOT NULL AUTO_INCREMENT,
  `FechaCreacion` DATETIME NOT NULL,
  `Estado` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`TrackingID`, `Identificacion`),
  CONSTRAINT `Identificacion`
    FOREIGN KEY (`Identificacion`)
    REFERENCES `TransportesCRPF`.`Conductor` (`Identificacion`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;

USE `TransportesCRPF`;

DELIMITER $$

USE `TransportesCRPF`$$
DROP TRIGGER IF EXISTS `TransportesCRPF`.`ConductorViaje_BEFORE_UPDATE` $$
USE `TransportesCRPF`$$
CREATE DEFINER = CURRENT_USER TRIGGER `TransportesCRPF`.`ConductorViaje_BEFORE_UPDATE` BEFORE UPDATE ON `ConductorViaje` FOR EACH ROW
BEGIN
	SET NEW.FechaCreacion=current_timestamp();
END$$


DELIMITER ;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
