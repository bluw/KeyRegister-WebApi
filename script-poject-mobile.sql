drop table `keyregisterdb`.`favorite`;
drop table `keyregisterdb`.`person`;
drop table `keyregisterdb`.`algorithm`;
drop table `keyregisterdb`.`company`

CREATE TABLE `keyregisterdb`.`algorithm` (
  `idAlgorithm` INT NOT NULL AUTO_INCREMENT,
  `type` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idAlgorithm`));

CREATE TABLE `keyregisterdb`.`company` (
  `idCompany` INT NOT NULL AUTO_INCREMENT,
  `nameCompany` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idCompany`));

CREATE TABLE `keyregisterdb`.`person` (
  `email` VARCHAR(45) NOT NULL,
  `password` VARCHAR(200) NOT NULL,
  `firstName` VARCHAR(45) NOT NULL,
  `lastName` VARCHAR(45) NOT NULL,
  `keyUsed` VARCHAR(45) NOT NULL,
  `keyLength` INT NOT NULL,
  `FK_company` INT NOT NULL,
  `FK_algorithm` INT NOT NULL,
  PRIMARY KEY (`email`),
  INDEX `FK_company_idx` (`FK_company` ASC),
  INDEX `FK_algorithm_idx` (`FK_algorithm` ASC),
  CONSTRAINT `FK_company`
    FOREIGN KEY (`FK_company`)
    REFERENCES `keyregisterdb`.`company` (`idCompany`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_algorithm`
    FOREIGN KEY (`FK_algorithm`)
    REFERENCES `keyregisterdb`.`algorithm` (`idAlgorithm`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


    CREATE TABLE `keyregisterdb`.`favorite` (
  `idFavorite` INT NOT NULL AUTO_INCREMENT,
  `personWithFavorite` VARCHAR(45) NOT NULL,
  `personFavorite` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`idFavorite`),
  INDEX `PersonWithFavorite_idx` (`personWithFavorite` ASC),
  INDEX `personFavorite_idx` (`personFavorite` ASC),
  CONSTRAINT `PersonWithFavorite`
    FOREIGN KEY (`personWithFavorite`)
    REFERENCES `keyregisterdb`.`person` (`email`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `personFavorite`
    FOREIGN KEY (`personFavorite`)
    REFERENCES `keyregisterdb`.`person` (`email`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

ALTER TABLE `keyregisterdb`.`algorithm` 
AUTO_INCREMENT = 1 ;

ALTER TABLE `keyregisterdb`.`company` 
AUTO_INCREMENT = 1 ;

ALTER TABLE `keyregisterdb`.`favorite` 
AUTO_INCREMENT = 1 ;

INSERT INTO `keyregisterdb`.`algorithm` (`type`) VALUES ('AES');
INSERT INTO `keyregisterdb`.`algorithm` (`type`) VALUES ('Twofish');
INSERT INTO `keyregisterdb`.`algorithm` (`type`) VALUES ('Blowfish');
INSERT INTO `keyregisterdb`.`algorithm` (`type`) VALUES ('RSA');
INSERT INTO `keyregisterdb`.`algorithm` (`type`) VALUES ('Triple DES');

INSERT INTO `keyregisterdb`.`person` (`email`, `password`, `firstName`, `lastName`, `keyUsed`, `keyLength`, `FK_company`, `FK_algorithm`) VALUES ('mister@roboto.com', 'qwerty', 'Powers', 'Austin', 'AEC:ASDADS', '256', '11', '1');
INSERT INTO `keyregisterdb`.`person` (`email`, `password`, `firstName`, `lastName`, `keyUsed`, `keyLength`, `FK_company`, `FK_algorithm`) VALUES ('cedric@bluw.com', 'azerty', 'Bluw', 'Cedric', 'ASD:ASDAS:A', '1024', '1', '21');
INSERT INTO `keyregisterdb`.`person` (`email`, `password`, `firstName`, `lastName`, `keyUsed`, `keyLength`, `FK_company`, `FK_algorithm`) VALUES ('xavier@test.com', '12345', 'Test', 'Xavier', 'JK:AS:US:UAL', '256', '1', '21');
INSERT INTO `keyregisterdb`.`company` (`nameCompany`) VALUES ('Henallux');
INSERT INTO `keyregisterdb`.`company` (`nameCompany`) VALUES ('Google');

INSERT INTO `keyregisterdb`.`person` (`email`, `password`, `firstName`, `lastName`, `keyUsed`, `keyLength`, `FK_company`, `FK_algorithm`) VALUES ('dupontJean@gmail.com', '080b03', 'Jean', 'Dupont', '125gjkfdls:dsg56', '1024', '12', '2');
INSERT INTO `keyregisterdb`.`person` (`email`, `password`, `firstName`, `lastName`, `keyUsed`, `keyLength`, `FK_company`, `FK_algorithm`) VALUES ('durantPierre@gmail.com', '090405', 'Pierre', 'Durant', '156464fkdjsl456', '2048', '2', '12');
INSERT INTO `keyregisterdb`.`person` (`email`, `password`, `firstName`, `lastName`, `keyUsed`, `keyLength`, `FK_company`, `FK_algorithm`) VALUES ('reperCedric@gmail.com', '080c', 'Cedric', 'Reper', '5445fdsklfg', '1024', '12', '22');
INSERT INTO `keyregisterdb`.`person` (`email`, `password`, `firstName`, `lastName`, `keyUsed`, `keyLength`, `FK_company`, `FK_algorithm`) VALUES ('dutrieueXavier@gmail.com', '050206', 'Xavier', 'Dutrieue', '546456fdqfdsjk', '2048', '12', '22');
INSERT INTO `keyregisterdb`.`favorite` (`personWithFavorite`, `personFavorite`) VALUES ('dupontJean@gmail.com', 'durantPierre@gmail.com');
INSERT INTO `keyregisterdb`.`favorite` (`personWithFavorite`, `personFavorite`) VALUES ('dupontJean@gmail.com', 'reperCedric@gmail.com');
INSERT INTO `keyregisterdb`.`favorite` (`personWithFavorite`, `personFavorite`) VALUES ('durantPierre@gmail.com', 'dutrieueXavier@gmail.com');
INSERT INTO `keyregisterdb`.`favorite` (`personWithFavorite`, `personFavorite`) VALUES ('reperCedric@gmail.com', 'dutrieueXavier@gmail.com');
INSERT INTO `keyregisterdb`.`favorite` (`personWithFavorite`, `personFavorite`) VALUES ('reperCedric@gmail.com', 'dupontJean@gmail.com');
INSERT INTO `keyregisterdb`.`favorite` (`personWithFavorite`, `personFavorite`) VALUES ('dutrieueXavier@gmail.com', 'dupontJean@gmail.com');
INSERT INTO `keyregisterdb`.`favorite` (`personWithFavorite`, `personFavorite`) VALUES ('dutrieueXavier@gmail.com', 'reperCedric@gmail.com');
