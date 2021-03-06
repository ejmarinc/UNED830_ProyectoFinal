USE [master]
GO

DROP DATABASE IF EXISTS [TransportesCR_PF]

CREATE DATABASE [TransportesCR_PF]
 GO
 
USE [TransportesCR_PF]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [TrasportesCR_PF].[Camion]
CREATE TABLE  Camion(
	Placa varchar(10) NOT NULL,
	Modelo varchar(4) NOT NULL, 
	Marca varchar(45) NOT NULL,
	CONSTRAINT PK_Camion PRIMARY KEY CLUSTERED (Placa)
	)
GO

DROP TABLE IF EXISTS [TransportesCR_PF].[Conductor]

CREATE TABLE Conductor(

	Identificacion varchar(20) NOT NULL,
	Nombre varchar(45) NOT NULL,
	PrimerApellido varchar(45) NOT NULL,
	SegundoApellido varchar(45) NOT NULL,
	Usuario varchar(45) NOT NULL,
	Contrasena varchar (45) NOT NULL,
	Validado varchar (10) NOT NULL,
	CONSTRAINT PK_Conductor PRIMARY KEY CLUSTERED (Identificacion)
	)
GO

DROP TABLE IF EXISTS [TransportesCR_PF].[ConductorCamion]

CREATE TABLE ConductorCamion(
		Placa varchar(10) NOT NULL,
		Identificacion varchar(20) NOT NULL,
		CONSTRAINT PK_ConductorCamion PRIMARY KEY CLUSTERED (Placa, Identificacion),

		)
GO

ALTER TABLE [dbo].[ConductorCamion]  WITH CHECK ADD  CONSTRAINT [FK_ConductorCamion_Camion] FOREIGN KEY([Placa])
REFERENCES [dbo].[Camion] ([Placa])
GO
ALTER TABLE [dbo].[ConductorCamion] CHECK CONSTRAINT [FK_ConductorCamion_Camion]
GO
ALTER TABLE [dbo].[ConductorCamion]  WITH CHECK ADD  CONSTRAINT [FK_ConductorCamion_Conductor] FOREIGN KEY([Identificacion])
REFERENCES [dbo].[Conductor] ([Identificacion])
GO
ALTER TABLE [dbo].[ConductorCamion] CHECK CONSTRAINT [FK_ConductorCamion_Conductor]
GO


DROP TABLE IF EXISTS [TransportesCR_PF].[ConductorViajes]
CREATE TABLE ConductorViajes(
		ViajeID int IDENTITY(1,1) NOT NULL,
	    Identificacion varchar(20) NOT NULL,
		LugarInicio varchar(45) NOT NULL,
		LugarFinalizacion varchar(45) NOT NULL,
		Carga varchar(45) NOT NULL,
		TiempoEstimado varchar(20) NOT NULL,
		FechaCreacion varchar(20) NOT NULL ,
		UbicacionActual varchar(45) NOT NULL,
		Estado varchar (45) NOT NULL,
	    CONSTRAINT PK_ConductorViajes PRIMARY KEY CLUSTERED (ViajeID),
		)
GO

ALTER TABLE [dbo].[ConductorViajes]  WITH CHECK ADD  CONSTRAINT [FK_ConductorViajes_Conductor] FOREIGN KEY([Identificacion])
REFERENCES [dbo].[Conductor] ([Identificacion])
GO
ALTER TABLE [dbo].[ConductorViajes] CHECK CONSTRAINT [FK_ConductorViajes_Conductor]
GO




USE [master]
GO
ALTER DATABASE [TransportesCR_PF] SET  READ_WRITE 
GO
