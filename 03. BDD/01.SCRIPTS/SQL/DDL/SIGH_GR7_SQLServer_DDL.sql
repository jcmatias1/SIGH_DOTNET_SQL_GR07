/*==============================================================*/
/* DBMS name:      Microsoft SQL Server 2016                    */
/*==============================================================*/
/*==============================================================*/
/* Proyecto:       SIGH - Hospital General Monster Salud        */
/*==============================================================*/
/*==============================================================*/
/* Created on:     02/11/2026 01:36                             */
/*==============================================================*/

IF DB_ID('SIGH_GR07') IS NULL
   CREATE DATABASE SIGH_GR69
go

USE SIGH_GR69
go

if exists (select 1 from sys.foreign_keys where name = 'FK_DEP_CAR')
   alter table PECAR_CARGO drop constraint FK_DEP_CAR
go

if exists (select 1 from sys.foreign_keys where name = 'FK_GEN_EMP')
   alter table PEEMP_EMPLE drop constraint FK_GEN_EMP
go

if exists (select 1 from sys.foreign_keys where name = 'FK_ESC_EMP')
   alter table PEEMP_EMPLE drop constraint FK_ESC_EMP
go

if exists (select 1 from sys.foreign_keys where name = 'FK_CAR_EMP')
   alter table PEEMP_EMPLE drop constraint FK_CAR_EMP
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_USU')
   alter table XEUSU_USUAR drop constraint FK_EMP_USU
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EST_USU')
   alter table XEUSU_USUAR drop constraint FK_EST_USU
go

if exists (select 1 from sys.foreign_keys where name = 'FK_SIS_OPC')
   alter table XEOPC_OPCIO drop constraint FK_SIS_OPC
go

if exists (select 1 from sys.foreign_keys where name = 'FK_USU_UXP')
   alter table XEUXP_USUPE drop constraint FK_USU_UXP
go

if exists (select 1 from sys.foreign_keys where name = 'FK_PER_UXP')
   alter table XEUXP_USUPE drop constraint FK_PER_UXP
go

if exists (select 1 from sys.foreign_keys where name = 'FK_PER_OXP')
   alter table XEOXP_OPCPE drop constraint FK_PER_OXP
go

if exists (select 1 from sys.foreign_keys where name = 'FK_OPC_OXP')
   alter table XEOXP_OPCPE drop constraint FK_OPC_OXP
go

if exists (select 1 from sys.foreign_keys where name = 'FK_USU_SES')
   alter table XESES_SESIO drop constraint FK_USU_SES
go

if exists (select 1 from sys.foreign_keys where name = 'FK_USU_AUD')
   alter table XEAUD_AUDIT drop constraint FK_USU_AUD
go

if exists (select 1 from sys.foreign_keys where name = 'FK_GEN_PAC')
   alter table GHPAC_PACIE drop constraint FK_GEN_PAC
go

if exists (select 1 from sys.foreign_keys where name = 'FK_ESC_PAC')
   alter table GHPAC_PACIE drop constraint FK_ESC_PAC
go

if exists (select 1 from sys.foreign_keys where name = 'FK_DEP_CON')
   alter table GHCON_CONSU drop constraint FK_DEP_CON
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_AGE')
   alter table GHAGE_AGEND drop constraint FK_EMP_AGE
go

if exists (select 1 from sys.foreign_keys where name = 'FK_ESP_AGE')
   alter table GHAGE_AGEND drop constraint FK_ESP_AGE
go

if exists (select 1 from sys.foreign_keys where name = 'FK_CON_AGE')
   alter table GHAGE_AGEND drop constraint FK_CON_AGE
go

if exists (select 1 from sys.foreign_keys where name = 'FK_AGE_TUR')
   alter table GHTUR_TURNO drop constraint FK_AGE_TUR
go

if exists (select 1 from sys.foreign_keys where name = 'FK_PAC_CIT')
   alter table GHCIT_CITA drop constraint FK_PAC_CIT
go

if exists (select 1 from sys.foreign_keys where name = 'FK_TUR_CIT')
   alter table GHCIT_CITA drop constraint FK_TUR_CIT
go

if exists (select 1 from sys.foreign_keys where name = 'FK_PAC_EPI')
   alter table GHEPI_EPISO drop constraint FK_PAC_EPI
go

if exists (select 1 from sys.foreign_keys where name = 'FK_CIT_EPI')
   alter table GHEPI_EPISO drop constraint FK_CIT_EPI
go

if exists (select 1 from sys.foreign_keys where name = 'FK_DEP_EPI')
   alter table GHEPI_EPISO drop constraint FK_DEP_EPI
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EPI_NOT')
   alter table GHNOT_NOTA drop constraint FK_EPI_NOT
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_NOT')
   alter table GHNOT_NOTA drop constraint FK_EMP_NOT
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EPI_DXE')
   alter table GHDXE_DIAEP drop constraint FK_EPI_DXE
go

if exists (select 1 from sys.foreign_keys where name = 'FK_CIE_DXE')
   alter table GHDXE_DIAEP drop constraint FK_CIE_DXE
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EPI_SIG')
   alter table GHSIG_SIGNO drop constraint FK_EPI_SIG
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_SIG')
   alter table GHSIG_SIGNO drop constraint FK_EMP_SIG
go

if exists (select 1 from sys.foreign_keys where name = 'FK_PAC_ALE')
   alter table GHALE_ALERG drop constraint FK_PAC_ALE
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EPI_DOC')
   alter table GHDOC_DOCUM drop constraint FK_EPI_DOC
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EPI_SOL')
   alter table GHSOL_SOLEX drop constraint FK_EPI_SOL
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_SOL')
   alter table GHSOL_SOLEX drop constraint FK_EMP_SOL
go

if exists (select 1 from sys.foreign_keys where name = 'FK_SOL_DSO')
   alter table GHDSO_DETSO drop constraint FK_SOL_DSO
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EXA_DSO')
   alter table GHDSO_DETSO drop constraint FK_EXA_DSO
go

if exists (select 1 from sys.foreign_keys where name = 'FK_DSO_RES')
   alter table GHRES_RESUL drop constraint FK_DSO_RES
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_RES')
   alter table GHRES_RESUL drop constraint FK_EMP_RES
go

if exists (select 1 from sys.foreign_keys where name = 'FK_DSO_INF')
   alter table GHINF_INFIM drop constraint FK_DSO_INF
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_INF')
   alter table GHINF_INFIM drop constraint FK_EMP_INF
go

if exists (select 1 from sys.foreign_keys where name = 'FK_MED_LOT')
   alter table GHLOT_LOTE drop constraint FK_MED_LOT
go

if exists (select 1 from sys.foreign_keys where name = 'FK_LOT_MOV')
   alter table GHMOV_MOVIN drop constraint FK_LOT_MOV
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_MOV')
   alter table GHMOV_MOVIN drop constraint FK_EMP_MOV
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EPI_REC')
   alter table GHREC_RECET drop constraint FK_EPI_REC
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_REC')
   alter table GHREC_RECET drop constraint FK_EMP_REC
go

if exists (select 1 from sys.foreign_keys where name = 'FK_REC_DRE')
   alter table GHDRE_DETRE drop constraint FK_REC_DRE
go

if exists (select 1 from sys.foreign_keys where name = 'FK_MED_DRE')
   alter table GHDRE_DETRE drop constraint FK_MED_DRE
go

if exists (select 1 from sys.foreign_keys where name = 'FK_DRE_DIS')
   alter table GHDIS_DISPE drop constraint FK_DRE_DIS
go

if exists (select 1 from sys.foreign_keys where name = 'FK_LOT_DIS')
   alter table GHDIS_DISPE drop constraint FK_LOT_DIS
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_DIS')
   alter table GHDIS_DISPE drop constraint FK_EMP_DIS
go

if exists (select 1 from sys.foreign_keys where name = 'FK_DEP_HAB')
   alter table GHHAB_HABIT drop constraint FK_DEP_HAB
go

if exists (select 1 from sys.foreign_keys where name = 'FK_HAB_CAM')
   alter table GHCAM_CAMA drop constraint FK_HAB_CAM
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EPI_HOS')
   alter table GHHOS_HOSPI drop constraint FK_EPI_HOS
go

if exists (select 1 from sys.foreign_keys where name = 'FK_CAM_HOS')
   alter table GHHOS_HOSPI drop constraint FK_CAM_HOS
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_HOS')
   alter table GHHOS_HOSPI drop constraint FK_EMP_HOS
go

if exists (select 1 from sys.foreign_keys where name = 'FK_HOS_TRA')
   alter table GHTRA_TRASL drop constraint FK_HOS_TRA
go

if exists (select 1 from sys.foreign_keys where name = 'FK_CAM_TRA')
   alter table GHTRA_TRASL drop constraint FK_CAM_TRA
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_TRA')
   alter table GHTRA_TRASL drop constraint FK_EMP_TRA
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EPI_CIR')
   alter table GHCIR_CIRUG drop constraint FK_EPI_CIR
go

if exists (select 1 from sys.foreign_keys where name = 'FK_SAL_CIR')
   alter table GHCIR_CIRUG drop constraint FK_SAL_CIR
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_CIR')
   alter table GHCIR_CIRUG drop constraint FK_EMP_CIR
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EPI_CUE')
   alter table FICUE_CUENT drop constraint FK_EPI_CUE
go

if exists (select 1 from sys.foreign_keys where name = 'FK_CUE_CRG')
   alter table FICAR_CARGO drop constraint FK_CUE_CRG
go

if exists (select 1 from sys.foreign_keys where name = 'FK_TAR_CRG')
   alter table FICAR_CARGO drop constraint FK_TAR_CRG
go

if exists (select 1 from sys.foreign_keys where name = 'FK_CUE_COM')
   alter table FICOM_COMPR drop constraint FK_CUE_COM
go

if exists (select 1 from sys.foreign_keys where name = 'FK_EMP_COM')
   alter table FICOM_COMPR drop constraint FK_EMP_COM
go

if exists (select 1 from sys.foreign_keys where name = 'FK_COM_PAG')
   alter table FIPAG_PAGO drop constraint FK_COM_PAG
go

if exists (select 1 from sys.foreign_keys where name = 'FK_FPA_PAG')
   alter table FIPAG_PAGO drop constraint FK_FPA_PAG
go

if exists (select 1 from sysobjects where id = object_id('FIPAG_PAGO') and type = 'U')
   drop table FIPAG_PAGO
go

if exists (select 1 from sysobjects where id = object_id('FIFPA_FORPA') and type = 'U')
   drop table FIFPA_FORPA
go

if exists (select 1 from sysobjects where id = object_id('FICOM_COMPR') and type = 'U')
   drop table FICOM_COMPR
go

if exists (select 1 from sysobjects where id = object_id('FICAR_CARGO') and type = 'U')
   drop table FICAR_CARGO
go

if exists (select 1 from sysobjects where id = object_id('FICUE_CUENT') and type = 'U')
   drop table FICUE_CUENT
go

if exists (select 1 from sysobjects where id = object_id('FITAR_TARIF') and type = 'U')
   drop table FITAR_TARIF
go

if exists (select 1 from sysobjects where id = object_id('GHCIR_CIRUG') and type = 'U')
   drop table GHCIR_CIRUG
go

if exists (select 1 from sysobjects where id = object_id('GHSAL_SALA') and type = 'U')
   drop table GHSAL_SALA
go

if exists (select 1 from sysobjects where id = object_id('GHTRA_TRASL') and type = 'U')
   drop table GHTRA_TRASL
go

if exists (select 1 from sysobjects where id = object_id('GHHOS_HOSPI') and type = 'U')
   drop table GHHOS_HOSPI
go

if exists (select 1 from sysobjects where id = object_id('GHCAM_CAMA') and type = 'U')
   drop table GHCAM_CAMA
go

if exists (select 1 from sysobjects where id = object_id('GHHAB_HABIT') and type = 'U')
   drop table GHHAB_HABIT
go

if exists (select 1 from sysobjects where id = object_id('GHDIS_DISPE') and type = 'U')
   drop table GHDIS_DISPE
go

if exists (select 1 from sysobjects where id = object_id('GHDRE_DETRE') and type = 'U')
   drop table GHDRE_DETRE
go

if exists (select 1 from sysobjects where id = object_id('GHREC_RECET') and type = 'U')
   drop table GHREC_RECET
go

if exists (select 1 from sysobjects where id = object_id('GHMOV_MOVIN') and type = 'U')
   drop table GHMOV_MOVIN
go

if exists (select 1 from sysobjects where id = object_id('GHLOT_LOTE') and type = 'U')
   drop table GHLOT_LOTE
go

if exists (select 1 from sysobjects where id = object_id('GHMED_MEDIC') and type = 'U')
   drop table GHMED_MEDIC
go

if exists (select 1 from sysobjects where id = object_id('GHINF_INFIM') and type = 'U')
   drop table GHINF_INFIM
go

if exists (select 1 from sysobjects where id = object_id('GHRES_RESUL') and type = 'U')
   drop table GHRES_RESUL
go

if exists (select 1 from sysobjects where id = object_id('GHDSO_DETSO') and type = 'U')
   drop table GHDSO_DETSO
go

if exists (select 1 from sysobjects where id = object_id('GHSOL_SOLEX') and type = 'U')
   drop table GHSOL_SOLEX
go

if exists (select 1 from sysobjects where id = object_id('GHEXA_EXAME') and type = 'U')
   drop table GHEXA_EXAME
go

if exists (select 1 from sysobjects where id = object_id('GHDOC_DOCUM') and type = 'U')
   drop table GHDOC_DOCUM
go

if exists (select 1 from sysobjects where id = object_id('GHALE_ALERG') and type = 'U')
   drop table GHALE_ALERG
go

if exists (select 1 from sysobjects where id = object_id('GHSIG_SIGNO') and type = 'U')
   drop table GHSIG_SIGNO
go

if exists (select 1 from sysobjects where id = object_id('GHDXE_DIAEP') and type = 'U')
   drop table GHDXE_DIAEP
go

if exists (select 1 from sysobjects where id = object_id('GHCIE_CIE10') and type = 'U')
   drop table GHCIE_CIE10
go

if exists (select 1 from sysobjects where id = object_id('GHNOT_NOTA') and type = 'U')
   drop table GHNOT_NOTA
go

if exists (select 1 from sysobjects where id = object_id('GHEPI_EPISO') and type = 'U')
   drop table GHEPI_EPISO
go

if exists (select 1 from sysobjects where id = object_id('GHCIT_CITA') and type = 'U')
   drop table GHCIT_CITA
go

if exists (select 1 from sysobjects where id = object_id('GHTUR_TURNO') and type = 'U')
   drop table GHTUR_TURNO
go

if exists (select 1 from sysobjects where id = object_id('GHAGE_AGEND') and type = 'U')
   drop table GHAGE_AGEND
go

if exists (select 1 from sysobjects where id = object_id('GHCON_CONSU') and type = 'U')
   drop table GHCON_CONSU
go

if exists (select 1 from sysobjects where id = object_id('GHESP_ESPEC') and type = 'U')
   drop table GHESP_ESPEC
go

if exists (select 1 from sysobjects where id = object_id('GHPAC_PACIE') and type = 'U')
   drop table GHPAC_PACIE
go

if exists (select 1 from sysobjects where id = object_id('XEAUD_AUDIT') and type = 'U')
   drop table XEAUD_AUDIT
go

if exists (select 1 from sysobjects where id = object_id('XESES_SESIO') and type = 'U')
   drop table XESES_SESIO
go

if exists (select 1 from sysobjects where id = object_id('XEOXP_OPCPE') and type = 'U')
   drop table XEOXP_OPCPE
go

if exists (select 1 from sysobjects where id = object_id('XEUXP_USUPE') and type = 'U')
   drop table XEUXP_USUPE
go

if exists (select 1 from sysobjects where id = object_id('XEUSU_USUAR') and type = 'U')
   drop table XEUSU_USUAR
go

if exists (select 1 from sysobjects where id = object_id('XEPER_PERFI') and type = 'U')
   drop table XEPER_PERFI
go

if exists (select 1 from sysobjects where id = object_id('XEOPC_OPCIO') and type = 'U')
   drop table XEOPC_OPCIO
go

if exists (select 1 from sysobjects where id = object_id('XESIS_SISTE') and type = 'U')
   drop table XESIS_SISTE
go

if exists (select 1 from sysobjects where id = object_id('XEEST_ESTAD') and type = 'U')
   drop table XEEST_ESTAD
go

if exists (select 1 from sysobjects where id = object_id('PEEMP_EMPLE') and type = 'U')
   drop table PEEMP_EMPLE
go

if exists (select 1 from sysobjects where id = object_id('PECAR_CARGO') and type = 'U')
   drop table PECAR_CARGO
go

if exists (select 1 from sysobjects where id = object_id('PEDEP_DEPAR') and type = 'U')
   drop table PEDEP_DEPAR
go

if exists (select 1 from sysobjects where id = object_id('PEESC_ESTCIV') and type = 'U')
   drop table PEESC_ESTCIV
go

if exists (select 1 from sysobjects where id = object_id('PEGEN_GENERO') and type = 'U')
   drop table PEGEN_GENERO
go

/*==============================================================*/
/* Table: PEGEN_GENERO                                          */
/*==============================================================*/
create table PEGEN_GENERO (
   PEGEN_CODIGO         char(1) not null,
   PEGEN_DESCRI         varchar(20) not null,
   constraint PK_PEGEN_GENERO primary key (PEGEN_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Catálogo de género o sexo de pacientes y empleados (RF-PAC-001).',
   'user', 'dbo', 'table', 'PEGEN_GENERO'
go

/*==============================================================*/
/* Table: PEESC_ESTCIV                                          */
/*==============================================================*/
create table PEESC_ESTCIV (
   PEESC_CODIGO         char(1) not null,
   PEESC_DESCRI         varchar(20) not null,
   constraint PK_PEESC_ESTCIV primary key (PEESC_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Catálogo de estado civil de pacientes y empleados.',
   'user', 'dbo', 'table', 'PEESC_ESTCIV'
go

/*==============================================================*/
/* Table: PEDEP_DEPAR                                           */
/*==============================================================*/
create table PEDEP_DEPAR (
   PEDEP_CODIGO         char(3) not null,
   PEDEP_DESCRI         varchar(60) not null,
   constraint PK_PEDEP_DEPAR primary key (PEDEP_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Departamento o servicio hospitalario (Consulta externa, Emergencia, Laboratorio, Imagenología, Farmacia, Hospitalización, Quirófano, Facturación).',
   'user', 'dbo', 'table', 'PEDEP_DEPAR'
go

/*==============================================================*/
/* Table: PECAR_CARGO                                           */
/*==============================================================*/
create table PECAR_CARGO (
   PEDEP_CODIGO         char(3) not null,
   PECAR_CODIGO         char(3) not null,
   PECAR_DESCRI         varchar(60) not null,
   constraint PK_PECAR_CARGO primary key (PEDEP_CODIGO, PECAR_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Cargo del personal dentro de un departamento (Médico, Enfermero, Farmacéutico, etc.).',
   'user', 'dbo', 'table', 'PECAR_CARGO'
go

/*==============================================================*/
/* Table: PEEMP_EMPLE                                           */
/*==============================================================*/
create table PEEMP_EMPLE (
   PEEMP_CODIGO         char(10) not null,
   PEGEN_CODIGO         char(1) not null,
   PEESC_CODIGO         char(1) null,
   PEDEP_CODIGO         char(3) not null,
   PECAR_CODIGO         char(3) not null,
   PEEMP_CEDULA         varchar(13) not null,
   PEEMP_APELLI         varchar(60) not null,
   PEEMP_NOMBRE         varchar(60) not null,
   PEEMP_FECNAC         date not null,
   PEEMP_EMAIL          varchar(100) not null,
   PEEMP_TELEFO         varchar(15) null,
   PEEMP_DIRECC         varchar(150) null,
   PEEMP_REGPRO         varchar(30) null,
   PEEMP_ESTADO         char(1) not null,
   constraint PK_PEEMP_EMPLE primary key (PEEMP_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Personal del hospital que usa el SIGH: médicos, enfermeros, admisión, laboratorio, imágenes, farmacia, hospitalización, quirófano, facturación, dirección y auditoría.',
   'user', 'dbo', 'table', 'PEEMP_EMPLE'
go

/*==============================================================*/
/* Table: XEEST_ESTAD                                           */
/*==============================================================*/
create table XEEST_ESTAD (
   XEEST_CODIGO         char(1) not null,
   XEEST_DESCRI         varchar(30) not null,
   constraint PK_XEEST_ESTAD primary key (XEEST_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Estado de una cuenta de usuario: activo, inactivo o bloqueado (RF-SEG-002).',
   'user', 'dbo', 'table', 'XEEST_ESTAD'
go

/*==============================================================*/
/* Table: XESIS_SISTE                                           */
/*==============================================================*/
create table XESIS_SISTE (
   XESIS_CODIGO         char(3) not null,
   XESIS_DESCRI         varchar(60) not null,
   constraint PK_XESIS_SISTE primary key (XESIS_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Módulo o subsistema del SIGH al que pertenecen las opciones (Seguridad, Pacientes, Citas, Atención clínica, etc.).',
   'user', 'dbo', 'table', 'XESIS_SISTE'
go

/*==============================================================*/
/* Table: XEOPC_OPCIO                                           */
/*==============================================================*/
create table XEOPC_OPCIO (
   XEOPC_CODIGO         char(4) not null,
   XESIS_CODIGO         char(3) not null,
   XEOPC_DESCRI         varchar(80) not null,
   XEOPC_URL            varchar(150) null,
   constraint PK_XEOPC_OPCIO primary key (XEOPC_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Opción o funcionalidad de un módulo sobre la que se conceden permisos (RF-SEG-003).',
   'user', 'dbo', 'table', 'XEOPC_OPCIO'
go

/*==============================================================*/
/* Table: XEPER_PERFI                                           */
/*==============================================================*/
create table XEPER_PERFI (
   XEPER_CODIGO         char(3) not null,
   XEPER_DESCRI         varchar(60) not null,
   XEPER_OBSERV         varchar(200) null,
   constraint PK_XEPER_PERFI primary key (XEPER_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Perfil o rol del usuario: Administrador, Médico, Enfermero, Admisión, etc. (RF-SEG-003).',
   'user', 'dbo', 'table', 'XEPER_PERFI'
go

/*==============================================================*/
/* Table: XEUSU_USUAR                                           */
/*==============================================================*/
create table XEUSU_USUAR (
   XEUSU_CODIGO         varchar(20) not null,
   PEEMP_CODIGO         char(10) not null,
   XEEST_CODIGO         char(1) not null,
   XEUSU_PASSWO         varchar(255) not null,
   XEUSU_INTFAL         smallint not null,
   XEUSU_FECCRE         datetime not null,
   XEUSU_FECMOD         datetime null,
   XEUSU_ULTACC         datetime null,
   XEUSU_PIEFIR         varchar(100) null,
   constraint PK_XEUSU_USUAR primary key (XEUSU_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Cuenta de acceso de un empleado al SIGH (RF-SEG-001, RF-SEG-002, RF-SEG-005).',
   'user', 'dbo', 'table', 'XEUSU_USUAR'
go

/*==============================================================*/
/* Table: XEUXP_USUPE                                           */
/*==============================================================*/
create table XEUXP_USUPE (
   XEUSU_CODIGO         varchar(20) not null,
   XEPER_CODIGO         char(3) not null,
   XEUXP_FECASI         datetime not null,
   XEUXP_FECRET         datetime null,
   constraint PK_XEUXP_USUPE primary key (XEUSU_CODIGO, XEPER_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Asignación de perfiles a usuarios con fecha de asignación y retiro (RF-SEG-003).',
   'user', 'dbo', 'table', 'XEUXP_USUPE'
go

/*==============================================================*/
/* Table: XEOXP_OPCPE                                           */
/*==============================================================*/
create table XEOXP_OPCPE (
   XEPER_CODIGO         char(3) not null,
   XEOPC_CODIGO         char(4) not null,
   XEOXP_FECASI         datetime not null,
   XEOXP_FECRET         datetime null,
   constraint PK_XEOXP_OPCPE primary key (XEPER_CODIGO, XEOPC_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Asignación de opciones (permisos) a perfiles (RF-SEG-003).',
   'user', 'dbo', 'table', 'XEOXP_OPCPE'
go

/*==============================================================*/
/* Table: XESES_SESIO                                           */
/*==============================================================*/
create table XESES_SESIO (
   XESES_CODIGO         bigint identity(1,1) not null,
   XEUSU_CODIGO         varchar(20) not null,
   XESES_FECINI         datetime not null,
   XESES_FECFIN         datetime null,
   XESES_DIRIP          varchar(45) null,
   XESES_ESTADO         char(1) not null,
   constraint PK_XESES_SESIO primary key (XESES_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Sesión de trabajo de un usuario: inicio, cierre y expiración (RF-SEG-004).',
   'user', 'dbo', 'table', 'XESES_SESIO'
go

/*==============================================================*/
/* Table: XEAUD_AUDIT                                           */
/*==============================================================*/
create table XEAUD_AUDIT (
   XEAUD_CODIGO         bigint identity(1,1) not null,
   XEUSU_CODIGO         varchar(20) null,
   XEAUD_FECHOR         datetime not null,
   XEAUD_MODULO         varchar(40) not null,
   XEAUD_OPERAC         varchar(40) not null,
   XEAUD_RESULT         varchar(15) not null,
   XEAUD_DETALL         varchar(500) null,
   XEAUD_DIRIP          varchar(45) null,
   constraint PK_XEAUD_AUDIT primary key (XEAUD_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Registro inalterable de eventos y accesos denegados del sistema (RF-AUD-001 a RF-AUD-004, RF-CAT-003).',
   'user', 'dbo', 'table', 'XEAUD_AUDIT'
go

/*==============================================================*/
/* Table: GHPAC_PACIE                                           */
/*==============================================================*/
create table GHPAC_PACIE (
   GHPAC_CODIGO         int identity(1,1) not null,
   PEGEN_CODIGO         char(1) not null,
   PEESC_CODIGO         char(1) null,
   GHPAC_TIPDOC         varchar(15) not null,
   GHPAC_NUMDOC         varchar(20) not null,
   GHPAC_APELLI         varchar(60) not null,
   GHPAC_NOMBRE         varchar(60) not null,
   GHPAC_FECNAC         date not null,
   GHPAC_DIRECC         varchar(150) null,
   GHPAC_TELEFO         varchar(15) null,
   GHPAC_EMAIL          varchar(100) null,
   GHPAC_CONEME         varchar(100) null,
   GHPAC_TELEME         varchar(15) null,
   GHPAC_FECREG         datetime not null,
   GHPAC_ESTADO         char(1) not null,
   constraint PK_GHPAC_PACIE primary key (GHPAC_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Paciente registrado con identificador hospitalario único (RF-PAC-001 a RF-PAC-005, CU-03).',
   'user', 'dbo', 'table', 'GHPAC_PACIE'
go

/*==============================================================*/
/* Table: GHESP_ESPEC                                           */
/*==============================================================*/
create table GHESP_ESPEC (
   GHESP_CODIGO         char(3) not null,
   GHESP_DESCRI         varchar(60) not null,
   constraint PK_GHESP_ESPEC primary key (GHESP_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Catálogo de especialidades médicas (RF-CIT-001, RF-CAT-001).',
   'user', 'dbo', 'table', 'GHESP_ESPEC'
go

/*==============================================================*/
/* Table: GHCON_CONSU                                           */
/*==============================================================*/
create table GHCON_CONSU (
   GHCON_CODIGO         char(4) not null,
   PEDEP_CODIGO         char(3) not null,
   GHCON_DESCRI         varchar(60) not null,
   GHCON_UBICAC         varchar(80) null,
   constraint PK_GHCON_CONSU primary key (GHCON_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Consultorio físico de un servicio donde se atienden las citas (RF-CIT-001).',
   'user', 'dbo', 'table', 'GHCON_CONSU'
go

/*==============================================================*/
/* Table: GHAGE_AGEND                                           */
/*==============================================================*/
create table GHAGE_AGEND (
   GHAGE_CODIGO         int identity(1,1) not null,
   PEEMP_CODIGO         char(10) not null,
   GHESP_CODIGO         char(3) not null,
   GHCON_CODIGO         char(4) not null,
   GHAGE_FECINI         date not null,
   GHAGE_FECFIN         date not null,
   GHAGE_DIAS           varchar(20) not null,
   GHAGE_HORINI         time not null,
   GHAGE_HORFIN         time not null,
   GHAGE_DURTUR         smallint not null,
   GHAGE_ESTADO         char(1) not null,
   constraint PK_GHAGE_AGEND primary key (GHAGE_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Agenda de un profesional por especialidad y consultorio (CU-05, RF-CIT-001).',
   'user', 'dbo', 'table', 'GHAGE_AGEND'
go

/*==============================================================*/
/* Table: GHTUR_TURNO                                           */
/*==============================================================*/
create table GHTUR_TURNO (
   GHTUR_CODIGO         int identity(1,1) not null,
   GHAGE_CODIGO         int not null,
   GHTUR_FECHA          date not null,
   GHTUR_HORA           time not null,
   GHTUR_ESTADO         char(1) not null,
   GHTUR_MOTBLO         varchar(150) null,
   constraint PK_GHTUR_TURNO primary key (GHTUR_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Turno generado por una agenda; puede estar disponible, reservado o bloqueado (CU-05, CU-06).',
   'user', 'dbo', 'table', 'GHTUR_TURNO'
go

/*==============================================================*/
/* Table: GHCIT_CITA                                            */
/*==============================================================*/
create table GHCIT_CITA (
   GHCIT_CODIGO         int identity(1,1) not null,
   GHPAC_CODIGO         int not null,
   GHTUR_CODIGO         int not null,
   GHCIT_MOTIVO         varchar(200) not null,
   GHCIT_ESTADO         varchar(15) not null,
   GHCIT_FECREG         datetime not null,
   GHCIT_MOTCAM         varchar(200) null,
   GHCIT_CITANT         int null,
   constraint PK_GHCIT_CITA primary key (GHCIT_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Cita médica de un paciente en un turno; registra reprogramación, cancelación e inasistencia (CU-06, CU-07, RF-CIT-002 a RF-CIT-006).',
   'user', 'dbo', 'table', 'GHCIT_CITA'
go

/*==============================================================*/
/* Table: GHEPI_EPISO                                           */
/*==============================================================*/
create table GHEPI_EPISO (
   GHEPI_CODIGO         int identity(1,1) not null,
   GHPAC_CODIGO         int not null,
   GHCIT_CODIGO         int null,
   PEDEP_CODIGO         char(3) not null,
   GHEPI_TIPO           varchar(20) not null,
   GHEPI_FECING         datetime not null,
   GHEPI_MOTIVO         varchar(250) not null,
   GHEPI_PRIORI         char(1) null,
   GHEPI_DISPOS         varchar(20) null,
   GHEPI_FECCIE         datetime null,
   GHEPI_ESTADO         varchar(15) not null,
   constraint PK_GHEPI_EPISO primary key (GHEPI_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Episodio de atención: consulta externa, emergencia, atención no programada u hospitalización (CU-04, CU-08, CU-09, RF-HC-001).',
   'user', 'dbo', 'table', 'GHEPI_EPISO'
go

/*==============================================================*/
/* Table: GHNOT_NOTA                                            */
/*==============================================================*/
create table GHNOT_NOTA (
   GHNOT_CODIGO         int identity(1,1) not null,
   GHEPI_CODIGO         int not null,
   PEEMP_CODIGO         char(10) not null,
   GHNOT_TIPO           varchar(15) not null,
   GHNOT_FECHA          datetime not null,
   GHNOT_MOTCON         text null,
   GHNOT_EXAFIS         text null,
   GHNOT_PLAN           text null,
   GHNOT_TEXTO          text null,
   GHNOT_ESTADO         varchar(15) not null,
   GHNOT_FECFIN         datetime null,
   constraint PK_GHNOT_NOTA primary key (GHNOT_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Nota clínica del episodio: consulta, evolución, cuidado de enfermería o adenda (CU-08, CU-10, RF-HC-002, RF-HC-004, RF-HC-007).',
   'user', 'dbo', 'table', 'GHNOT_NOTA'
go

/*==============================================================*/
/* Table: GHCIE_CIE10                                           */
/*==============================================================*/
create table GHCIE_CIE10 (
   GHCIE_CODIGO         varchar(10) not null,
   GHCIE_DESCRI         varchar(250) not null,
   constraint PK_GHCIE_CIE10 primary key (GHCIE_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Catálogo de diagnósticos codificados CIE-10 (RF-HC-003).',
   'user', 'dbo', 'table', 'GHCIE_CIE10'
go

/*==============================================================*/
/* Table: GHDXE_DIAEP                                           */
/*==============================================================*/
create table GHDXE_DIAEP (
   GHEPI_CODIGO         int not null,
   GHCIE_CODIGO         varchar(10) not null,
   GHDXE_TIPO           varchar(15) not null,
   GHDXE_CONDIC         varchar(15) not null,
   GHDXE_FECHA          datetime not null,
   constraint PK_GHDXE_DIAEP primary key (GHEPI_CODIGO, GHCIE_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Diagnóstico registrado en un episodio: principal o secundario, presuntivo o definitivo (RF-HC-003).',
   'user', 'dbo', 'table', 'GHDXE_DIAEP'
go

/*==============================================================*/
/* Table: GHSIG_SIGNO                                           */
/*==============================================================*/
create table GHSIG_SIGNO (
   GHSIG_CODIGO         int identity(1,1) not null,
   GHEPI_CODIGO         int not null,
   PEEMP_CODIGO         char(10) not null,
   GHSIG_FECHA          datetime not null,
   GHSIG_TEMPER         decimal(4,1) null,
   GHSIG_PRESIS         smallint null,
   GHSIG_PREDIA         smallint null,
   GHSIG_FRECAR         smallint null,
   GHSIG_FRERES         smallint null,
   GHSIG_SATOXI         smallint null,
   GHSIG_PESO           decimal(5,2) null,
   GHSIG_TALLA          decimal(5,2) null,
   GHSIG_OBSERV         varchar(250) null,
   constraint PK_GHSIG_SIGNO primary key (GHSIG_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Medición de signos vitales registrada por enfermería (CU-10, RF-HC-002).',
   'user', 'dbo', 'table', 'GHSIG_SIGNO'
go

/*==============================================================*/
/* Table: GHALE_ALERG                                           */
/*==============================================================*/
create table GHALE_ALERG (
   GHALE_CODIGO         int identity(1,1) not null,
   GHPAC_CODIGO         int not null,
   GHALE_SUSTAN         varchar(100) not null,
   GHALE_REACCI         varchar(200) null,
   GHALE_SEVERI         varchar(15) null,
   GHALE_FUENTE         varchar(50) null,
   GHALE_ESTVER         varchar(20) not null,
   GHALE_FECREG         datetime not null,
   constraint PK_GHALE_ALERG primary key (GHALE_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Alergia del paciente mostrada como alerta en la atención y la prescripción (CU-11, RF-HC-006).',
   'user', 'dbo', 'table', 'GHALE_ALERG'
go

/*==============================================================*/
/* Table: GHDOC_DOCUM                                           */
/*==============================================================*/
create table GHDOC_DOCUM (
   GHDOC_CODIGO         int identity(1,1) not null,
   GHEPI_CODIGO         int not null,
   GHDOC_NOMBRE         varchar(150) not null,
   GHDOC_TIPO           varchar(30) not null,
   GHDOC_RUTA           varchar(255) not null,
   GHDOC_FECCAR         datetime not null,
   constraint PK_GHDOC_DOCUM primary key (GHDOC_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Documento clínico digitalizado adjunto a un episodio (RF-HC-008).',
   'user', 'dbo', 'table', 'GHDOC_DOCUM'
go

/*==============================================================*/
/* Table: GHEXA_EXAME                                           */
/*==============================================================*/
create table GHEXA_EXAME (
   GHEXA_CODIGO         char(6) not null,
   GHEXA_DESCRI         varchar(120) not null,
   GHEXA_TIPO           varchar(15) not null,
   GHEXA_UNIDAD         varchar(20) null,
   GHEXA_VALREF         varchar(100) null,
   GHEXA_ESTADO         char(1) not null,
   constraint PK_GHEXA_EXAME primary key (GHEXA_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Catálogo de exámenes de laboratorio y estudios de imagen (CU-12, RF-LAB-001, RF-IMG-001).',
   'user', 'dbo', 'table', 'GHEXA_EXAME'
go

/*==============================================================*/
/* Table: GHSOL_SOLEX                                           */
/*==============================================================*/
create table GHSOL_SOLEX (
   GHSOL_CODIGO         int identity(1,1) not null,
   GHEPI_CODIGO         int not null,
   PEEMP_CODIGO         char(10) not null,
   GHSOL_FECHA          datetime not null,
   GHSOL_PRIORI         varchar(10) not null,
   GHSOL_INDCLI         varchar(250) not null,
   GHSOL_ESTADO         varchar(20) not null,
   constraint PK_GHSOL_SOLEX primary key (GHSOL_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Solicitud de exámenes realizada por el médico dentro de un episodio (CU-12).',
   'user', 'dbo', 'table', 'GHSOL_SOLEX'
go

/*==============================================================*/
/* Table: GHDSO_DETSO                                           */
/*==============================================================*/
create table GHDSO_DETSO (
   GHSOL_CODIGO         int not null,
   GHEXA_CODIGO         char(6) not null,
   GHDSO_IDMUES         varchar(20) null,
   GHDSO_FECREC         datetime null,
   GHDSO_ESTADO         varchar(20) not null,
   constraint PK_GHDSO_DETSO primary key (GHSOL_CODIGO, GHEXA_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Examen solicitado dentro de una solicitud; controla la muestra y su estado (CU-13, RF-LAB-002).',
   'user', 'dbo', 'table', 'GHDSO_DETSO'
go

/*==============================================================*/
/* Table: GHRES_RESUL                                           */
/*==============================================================*/
create table GHRES_RESUL (
   GHRES_CODIGO         int identity(1,1) not null,
   GHSOL_CODIGO         int not null,
   GHEXA_CODIGO         char(6) not null,
   PEEMP_CODIGO         char(10) null,
   GHRES_VALOR          varchar(60) not null,
   GHRES_UNIDAD         varchar(20) null,
   GHRES_VALREF         varchar(100) null,
   GHRES_CRITIC         char(1) not null,
   GHRES_ESTADO         varchar(15) not null,
   GHRES_FECREG         datetime not null,
   GHRES_FECVAL         datetime null,
   constraint PK_GHRES_RESUL primary key (GHRES_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Resultado de laboratorio, preliminar o validado (CU-13, RF-LAB-003 a RF-LAB-005).',
   'user', 'dbo', 'table', 'GHRES_RESUL'
go

/*==============================================================*/
/* Table: GHINF_INFIM                                           */
/*==============================================================*/
create table GHINF_INFIM (
   GHINF_CODIGO         int identity(1,1) not null,
   GHSOL_CODIGO         int not null,
   GHEXA_CODIGO         char(6) not null,
   PEEMP_CODIGO         char(10) not null,
   GHINF_FECPRO         datetime null,
   GHINF_FECEJE         datetime null,
   GHINF_TEXTO          text null,
   GHINF_REFIMG         varchar(255) null,
   GHINF_VERSIO         smallint not null,
   GHINF_ESTADO         varchar(15) not null,
   constraint PK_GHINF_INFIM primary key (GHINF_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Informe de un estudio de imagenología con versión y referencia a imágenes (CU-14, RF-IMG-002 a RF-IMG-004).',
   'user', 'dbo', 'table', 'GHINF_INFIM'
go

/*==============================================================*/
/* Table: GHMED_MEDIC                                           */
/*==============================================================*/
create table GHMED_MEDIC (
   GHMED_CODIGO         char(8) not null,
   GHMED_NOMGEN         varchar(100) not null,
   GHMED_PRIACT         varchar(100) not null,
   GHMED_PRESEN         varchar(50) not null,
   GHMED_CONCEN         varchar(30) not null,
   GHMED_FORFAR         varchar(30) not null,
   GHMED_UNIDAD         varchar(15) not null,
   GHMED_STKMIN         int not null,
   GHMED_ESTADO         char(1) not null,
   constraint PK_GHMED_MEDIC primary key (GHMED_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Catálogo de medicamentos (CU-15, CU-17, RF-FAR-001).',
   'user', 'dbo', 'table', 'GHMED_MEDIC'
go

/*==============================================================*/
/* Table: GHLOT_LOTE                                            */
/*==============================================================*/
create table GHLOT_LOTE (
   GHLOT_CODIGO         int identity(1,1) not null,
   GHMED_CODIGO         char(8) not null,
   GHLOT_NUMERO         varchar(20) not null,
   GHLOT_FECCAD         date not null,
   GHLOT_CANTID         int not null,
   GHLOT_UBICAC         varchar(50) null,
   GHLOT_ESTADO         varchar(15) not null,
   constraint PK_GHLOT_LOTE primary key (GHLOT_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Lote de un medicamento con fecha de caducidad, ubicación y existencias (CU-17, RF-FAR-002).',
   'user', 'dbo', 'table', 'GHLOT_LOTE'
go

/*==============================================================*/
/* Table: GHMOV_MOVIN                                           */
/*==============================================================*/
create table GHMOV_MOVIN (
   GHMOV_CODIGO         int identity(1,1) not null,
   GHLOT_CODIGO         int not null,
   PEEMP_CODIGO         char(10) not null,
   GHMOV_TIPO           varchar(15) not null,
   GHMOV_CANTID         int not null,
   GHMOV_FECHA          datetime not null,
   GHMOV_MOTIVO         varchar(200) null,
   GHMOV_DOCRES         varchar(50) null,
   constraint PK_GHMOV_MOVIN primary key (GHMOV_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Movimiento de inventario: ingreso, ajuste o dispensación (CU-17, RF-FAR-005 a RF-FAR-007).',
   'user', 'dbo', 'table', 'GHMOV_MOVIN'
go

/*==============================================================*/
/* Table: GHREC_RECET                                           */
/*==============================================================*/
create table GHREC_RECET (
   GHREC_CODIGO         int identity(1,1) not null,
   GHEPI_CODIGO         int not null,
   PEEMP_CODIGO         char(10) not null,
   GHREC_FECHA          datetime not null,
   GHREC_ESTADO         varchar(15) not null,
   GHREC_JUSALE         varchar(250) null,
   constraint PK_GHREC_RECET primary key (GHREC_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Receta médica emitida en un episodio y enviada a farmacia (CU-15, RF-FAR-003).',
   'user', 'dbo', 'table', 'GHREC_RECET'
go

/*==============================================================*/
/* Table: GHDRE_DETRE                                           */
/*==============================================================*/
create table GHDRE_DETRE (
   GHREC_CODIGO         int not null,
   GHMED_CODIGO         char(8) not null,
   GHDRE_DOSIS          varchar(50) not null,
   GHDRE_VIA            varchar(30) not null,
   GHDRE_FRECUE         varchar(50) not null,
   GHDRE_DURACI         varchar(30) not null,
   GHDRE_CANTID         int not null,
   GHDRE_CANDIS         int not null,
   GHDRE_INDICA         varchar(250) null,
   constraint PK_GHDRE_DETRE primary key (GHREC_CODIGO, GHMED_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Medicamento prescrito en una receta con dosis, vía, frecuencia y duración (CU-15).',
   'user', 'dbo', 'table', 'GHDRE_DETRE'
go

/*==============================================================*/
/* Table: GHDIS_DISPE                                           */
/*==============================================================*/
create table GHDIS_DISPE (
   GHDIS_CODIGO         int identity(1,1) not null,
   GHREC_CODIGO         int not null,
   GHMED_CODIGO         char(8) not null,
   GHLOT_CODIGO         int not null,
   PEEMP_CODIGO         char(10) not null,
   GHDIS_CANTID         int not null,
   GHDIS_FECHA          datetime not null,
   constraint PK_GHDIS_DISPE primary key (GHDIS_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Entrega de un medicamento recetado desde un lote; permite entregas parciales (CU-16, RF-FAR-004).',
   'user', 'dbo', 'table', 'GHDIS_DISPE'
go

/*==============================================================*/
/* Table: GHHAB_HABIT                                           */
/*==============================================================*/
create table GHHAB_HABIT (
   GHHAB_CODIGO         char(5) not null,
   PEDEP_CODIGO         char(3) not null,
   GHHAB_PISO           smallint not null,
   GHHAB_TIPO           varchar(20) not null,
   constraint PK_GHHAB_HABIT primary key (GHHAB_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Habitación de un servicio de hospitalización (CU-18, RF-HOS-001).',
   'user', 'dbo', 'table', 'GHHAB_HABIT'
go

/*==============================================================*/
/* Table: GHCAM_CAMA                                            */
/*==============================================================*/
create table GHCAM_CAMA (
   GHCAM_CODIGO         char(6) not null,
   GHHAB_CODIGO         char(5) not null,
   GHCAM_ESTADO         varchar(20) not null,
   GHCAM_RESTRI         varchar(50) null,
   constraint PK_GHCAM_CAMA primary key (GHCAM_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Cama de una habitación con su estado operativo (CU-18, RF-HOS-001).',
   'user', 'dbo', 'table', 'GHCAM_CAMA'
go

/*==============================================================*/
/* Table: GHHOS_HOSPI                                           */
/*==============================================================*/
create table GHHOS_HOSPI (
   GHHOS_CODIGO         int identity(1,1) not null,
   GHEPI_CODIGO         int not null,
   GHCAM_CODIGO         char(6) null,
   PEEMP_CODIGO         char(10) not null,
   GHHOS_FECSOL         datetime not null,
   GHHOS_FECING         datetime null,
   GHHOS_ESTADO         varchar(20) not null,
   GHHOS_FECALM         datetime null,
   GHHOS_INDALT         text null,
   GHHOS_FECALA         datetime null,
   GHHOS_CAUEGR         varchar(30) null,
   constraint PK_GHHOS_HOSPI primary key (GHHOS_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Hospitalización de un paciente: solicitud, ingreso, alta médica y alta administrativa (CU-18, CU-19, RF-HOS-002 a RF-HOS-006).',
   'user', 'dbo', 'table', 'GHHOS_HOSPI'
go

/*==============================================================*/
/* Table: GHTRA_TRASL                                           */
/*==============================================================*/
create table GHTRA_TRASL (
   GHTRA_CODIGO         int identity(1,1) not null,
   GHHOS_CODIGO         int not null,
   GHCAM_CODIGO         char(6) not null,
   PEEMP_CODIGO         char(10) not null,
   GHTRA_CAMORI         char(6) not null,
   GHTRA_FECHA          datetime not null,
   GHTRA_MOTIVO         varchar(200) not null,
   constraint PK_GHTRA_TRASL primary key (GHTRA_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Traslado de un paciente hospitalizado a otra cama (CU-19, RF-HOS-004).',
   'user', 'dbo', 'table', 'GHTRA_TRASL'
go

/*==============================================================*/
/* Table: GHSAL_SALA                                            */
/*==============================================================*/
create table GHSAL_SALA (
   GHSAL_CODIGO         char(4) not null,
   GHSAL_DESCRI         varchar(60) not null,
   GHSAL_ESTADO         char(1) not null,
   constraint PK_GHSAL_SALA primary key (GHSAL_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Sala de quirófano (CU-20, RF-QUI-001).',
   'user', 'dbo', 'table', 'GHSAL_SALA'
go

/*==============================================================*/
/* Table: GHCIR_CIRUG                                           */
/*==============================================================*/
create table GHCIR_CIRUG (
   GHCIR_CODIGO         int identity(1,1) not null,
   GHEPI_CODIGO         int not null,
   GHSAL_CODIGO         char(4) not null,
   PEEMP_CODIGO         char(10) not null,
   GHCIR_PROCED         varchar(200) not null,
   GHCIR_EQUIPO         varchar(250) not null,
   GHCIR_FECPRO         datetime not null,
   GHCIR_HORINI         datetime null,
   GHCIR_HORFIN         datetime null,
   GHCIR_LISVER         char(1) not null,
   GHCIR_DETALL         text null,
   GHCIR_ESTADO         varchar(15) not null,
   GHCIR_MOTSUS         varchar(200) null,
   constraint PK_GHCIR_CIRUG primary key (GHCIR_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Cirugía programada y su registro operatorio (CU-20, RF-QUI-001 a RF-QUI-004).',
   'user', 'dbo', 'table', 'GHCIR_CIRUG'
go

/*==============================================================*/
/* Table: FITAR_TARIF                                           */
/*==============================================================*/
create table FITAR_TARIF (
   FITAR_CODIGO         char(6) not null,
   FITAR_DESCRI         varchar(120) not null,
   FITAR_TIPO           varchar(20) not null,
   FITAR_VALOR          decimal(10,2) not null,
   FITAR_FECVIG         date not null,
   FITAR_ESTADO         char(1) not null,
   constraint PK_FITAR_TARIF primary key (FITAR_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Tarifa vigente de un servicio: consulta, examen, procedimiento, medicamento o estancia (CU-21, RF-FAC-001, RF-CAT-002).',
   'user', 'dbo', 'table', 'FITAR_TARIF'
go

/*==============================================================*/
/* Table: FICUE_CUENT                                           */
/*==============================================================*/
create table FICUE_CUENT (
   FICUE_CODIGO         int identity(1,1) not null,
   GHEPI_CODIGO         int not null,
   FICUE_FECAPE         datetime not null,
   FICUE_TOTAL          decimal(12,2) not null,
   FICUE_SALDO          decimal(12,2) not null,
   FICUE_ESTADO         varchar(15) not null,
   constraint PK_FICUE_CUENT primary key (FICUE_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Cuenta del paciente asociada a un episodio de atención (CU-21, RF-FAC-001, RF-FAC-006).',
   'user', 'dbo', 'table', 'FICUE_CUENT'
go

/*==============================================================*/
/* Table: FICAR_CARGO                                           */
/*==============================================================*/
create table FICAR_CARGO (
   FICAR_CODIGO         int identity(1,1) not null,
   FICUE_CODIGO         int not null,
   FITAR_CODIGO         char(6) not null,
   FICAR_CANTID         decimal(8,2) not null,
   FICAR_VALUNI         decimal(10,2) not null,
   FICAR_SUBTOT         decimal(12,2) not null,
   FICAR_FECHA          datetime not null,
   constraint PK_FICAR_CARGO primary key (FICAR_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Cargo de un servicio a la cuenta del paciente (CU-21, RF-FAC-002).',
   'user', 'dbo', 'table', 'FICAR_CARGO'
go

/*==============================================================*/
/* Table: FICOM_COMPR                                           */
/*==============================================================*/
create table FICOM_COMPR (
   FICOM_CODIGO         int identity(1,1) not null,
   FICUE_CODIGO         int not null,
   PEEMP_CODIGO         char(10) not null,
   FICOM_NUMERO         varchar(20) not null,
   FICOM_FECHA          datetime not null,
   FICOM_SUBTOT         decimal(12,2) not null,
   FICOM_IVA            decimal(12,2) not null,
   FICOM_TOTAL          decimal(12,2) not null,
   FICOM_ESTADO         varchar(15) not null,
   FICOM_MOTANU         varchar(200) null,
   constraint PK_FICOM_COMPR primary key (FICOM_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Comprobante de venta emitido sobre una cuenta; puede anularse con motivo (CU-21, RF-FAC-003, RF-FAC-005).',
   'user', 'dbo', 'table', 'FICOM_COMPR'
go

/*==============================================================*/
/* Table: FIFPA_FORPA                                           */
/*==============================================================*/
create table FIFPA_FORPA (
   FIFPA_CODIGO         char(2) not null,
   FIFPA_DESCRI         varchar(40) not null,
   constraint PK_FIFPA_FORPA primary key (FIFPA_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Catálogo de formas de pago (efectivo, tarjeta, transferencia, seguro).',
   'user', 'dbo', 'table', 'FIFPA_FORPA'
go

/*==============================================================*/
/* Table: FIPAG_PAGO                                            */
/*==============================================================*/
create table FIPAG_PAGO (
   FIPAG_CODIGO         int identity(1,1) not null,
   FICOM_CODIGO         int not null,
   FIFPA_CODIGO         char(2) not null,
   FIPAG_IMPORT         decimal(12,2) not null,
   FIPAG_FECHA          datetime not null,
   FIPAG_REFERE         varchar(50) null,
   constraint PK_FIPAG_PAGO primary key (FIPAG_CODIGO)
)
go

execute sp_addextendedproperty 'MS_Description', 
   'Pago registrado sobre un comprobante; admite pagos parciales (CU-21, RF-FAC-004).',
   'user', 'dbo', 'table', 'FIPAG_PAGO'
go

alter table PECAR_CARGO
   add constraint FK_DEP_CAR foreign key (PEDEP_CODIGO)
      references PEDEP_DEPAR (PEDEP_CODIGO)
go

alter table PEEMP_EMPLE
   add constraint FK_GEN_EMP foreign key (PEGEN_CODIGO)
      references PEGEN_GENERO (PEGEN_CODIGO)
go

alter table PEEMP_EMPLE
   add constraint FK_ESC_EMP foreign key (PEESC_CODIGO)
      references PEESC_ESTCIV (PEESC_CODIGO)
go

alter table PEEMP_EMPLE
   add constraint FK_CAR_EMP foreign key (PEDEP_CODIGO, PECAR_CODIGO)
      references PECAR_CARGO (PEDEP_CODIGO, PECAR_CODIGO)
go

alter table XEUSU_USUAR
   add constraint FK_EMP_USU foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table XEUSU_USUAR
   add constraint FK_EST_USU foreign key (XEEST_CODIGO)
      references XEEST_ESTAD (XEEST_CODIGO)
go

alter table XEOPC_OPCIO
   add constraint FK_SIS_OPC foreign key (XESIS_CODIGO)
      references XESIS_SISTE (XESIS_CODIGO)
go

alter table XEUXP_USUPE
   add constraint FK_USU_UXP foreign key (XEUSU_CODIGO)
      references XEUSU_USUAR (XEUSU_CODIGO)
go

alter table XEUXP_USUPE
   add constraint FK_PER_UXP foreign key (XEPER_CODIGO)
      references XEPER_PERFI (XEPER_CODIGO)
go

alter table XEOXP_OPCPE
   add constraint FK_PER_OXP foreign key (XEPER_CODIGO)
      references XEPER_PERFI (XEPER_CODIGO)
go

alter table XEOXP_OPCPE
   add constraint FK_OPC_OXP foreign key (XEOPC_CODIGO)
      references XEOPC_OPCIO (XEOPC_CODIGO)
go

alter table XESES_SESIO
   add constraint FK_USU_SES foreign key (XEUSU_CODIGO)
      references XEUSU_USUAR (XEUSU_CODIGO)
go

alter table XEAUD_AUDIT
   add constraint FK_USU_AUD foreign key (XEUSU_CODIGO)
      references XEUSU_USUAR (XEUSU_CODIGO)
go

alter table GHPAC_PACIE
   add constraint FK_GEN_PAC foreign key (PEGEN_CODIGO)
      references PEGEN_GENERO (PEGEN_CODIGO)
go

alter table GHPAC_PACIE
   add constraint FK_ESC_PAC foreign key (PEESC_CODIGO)
      references PEESC_ESTCIV (PEESC_CODIGO)
go

alter table GHCON_CONSU
   add constraint FK_DEP_CON foreign key (PEDEP_CODIGO)
      references PEDEP_DEPAR (PEDEP_CODIGO)
go

alter table GHAGE_AGEND
   add constraint FK_EMP_AGE foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHAGE_AGEND
   add constraint FK_ESP_AGE foreign key (GHESP_CODIGO)
      references GHESP_ESPEC (GHESP_CODIGO)
go

alter table GHAGE_AGEND
   add constraint FK_CON_AGE foreign key (GHCON_CODIGO)
      references GHCON_CONSU (GHCON_CODIGO)
go

alter table GHTUR_TURNO
   add constraint FK_AGE_TUR foreign key (GHAGE_CODIGO)
      references GHAGE_AGEND (GHAGE_CODIGO)
go

alter table GHCIT_CITA
   add constraint FK_PAC_CIT foreign key (GHPAC_CODIGO)
      references GHPAC_PACIE (GHPAC_CODIGO)
go

alter table GHCIT_CITA
   add constraint FK_TUR_CIT foreign key (GHTUR_CODIGO)
      references GHTUR_TURNO (GHTUR_CODIGO)
go

alter table GHEPI_EPISO
   add constraint FK_PAC_EPI foreign key (GHPAC_CODIGO)
      references GHPAC_PACIE (GHPAC_CODIGO)
go

alter table GHEPI_EPISO
   add constraint FK_CIT_EPI foreign key (GHCIT_CODIGO)
      references GHCIT_CITA (GHCIT_CODIGO)
go

alter table GHEPI_EPISO
   add constraint FK_DEP_EPI foreign key (PEDEP_CODIGO)
      references PEDEP_DEPAR (PEDEP_CODIGO)
go

alter table GHNOT_NOTA
   add constraint FK_EPI_NOT foreign key (GHEPI_CODIGO)
      references GHEPI_EPISO (GHEPI_CODIGO)
go

alter table GHNOT_NOTA
   add constraint FK_EMP_NOT foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHDXE_DIAEP
   add constraint FK_EPI_DXE foreign key (GHEPI_CODIGO)
      references GHEPI_EPISO (GHEPI_CODIGO)
go

alter table GHDXE_DIAEP
   add constraint FK_CIE_DXE foreign key (GHCIE_CODIGO)
      references GHCIE_CIE10 (GHCIE_CODIGO)
go

alter table GHSIG_SIGNO
   add constraint FK_EPI_SIG foreign key (GHEPI_CODIGO)
      references GHEPI_EPISO (GHEPI_CODIGO)
go

alter table GHSIG_SIGNO
   add constraint FK_EMP_SIG foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHALE_ALERG
   add constraint FK_PAC_ALE foreign key (GHPAC_CODIGO)
      references GHPAC_PACIE (GHPAC_CODIGO)
go

alter table GHDOC_DOCUM
   add constraint FK_EPI_DOC foreign key (GHEPI_CODIGO)
      references GHEPI_EPISO (GHEPI_CODIGO)
go

alter table GHSOL_SOLEX
   add constraint FK_EPI_SOL foreign key (GHEPI_CODIGO)
      references GHEPI_EPISO (GHEPI_CODIGO)
go

alter table GHSOL_SOLEX
   add constraint FK_EMP_SOL foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHDSO_DETSO
   add constraint FK_SOL_DSO foreign key (GHSOL_CODIGO)
      references GHSOL_SOLEX (GHSOL_CODIGO)
go

alter table GHDSO_DETSO
   add constraint FK_EXA_DSO foreign key (GHEXA_CODIGO)
      references GHEXA_EXAME (GHEXA_CODIGO)
go

alter table GHRES_RESUL
   add constraint FK_DSO_RES foreign key (GHSOL_CODIGO, GHEXA_CODIGO)
      references GHDSO_DETSO (GHSOL_CODIGO, GHEXA_CODIGO)
go

alter table GHRES_RESUL
   add constraint FK_EMP_RES foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHINF_INFIM
   add constraint FK_DSO_INF foreign key (GHSOL_CODIGO, GHEXA_CODIGO)
      references GHDSO_DETSO (GHSOL_CODIGO, GHEXA_CODIGO)
go

alter table GHINF_INFIM
   add constraint FK_EMP_INF foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHLOT_LOTE
   add constraint FK_MED_LOT foreign key (GHMED_CODIGO)
      references GHMED_MEDIC (GHMED_CODIGO)
go

alter table GHMOV_MOVIN
   add constraint FK_LOT_MOV foreign key (GHLOT_CODIGO)
      references GHLOT_LOTE (GHLOT_CODIGO)
go

alter table GHMOV_MOVIN
   add constraint FK_EMP_MOV foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHREC_RECET
   add constraint FK_EPI_REC foreign key (GHEPI_CODIGO)
      references GHEPI_EPISO (GHEPI_CODIGO)
go

alter table GHREC_RECET
   add constraint FK_EMP_REC foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHDRE_DETRE
   add constraint FK_REC_DRE foreign key (GHREC_CODIGO)
      references GHREC_RECET (GHREC_CODIGO)
go

alter table GHDRE_DETRE
   add constraint FK_MED_DRE foreign key (GHMED_CODIGO)
      references GHMED_MEDIC (GHMED_CODIGO)
go

alter table GHDIS_DISPE
   add constraint FK_DRE_DIS foreign key (GHREC_CODIGO, GHMED_CODIGO)
      references GHDRE_DETRE (GHREC_CODIGO, GHMED_CODIGO)
go

alter table GHDIS_DISPE
   add constraint FK_LOT_DIS foreign key (GHLOT_CODIGO)
      references GHLOT_LOTE (GHLOT_CODIGO)
go

alter table GHDIS_DISPE
   add constraint FK_EMP_DIS foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHHAB_HABIT
   add constraint FK_DEP_HAB foreign key (PEDEP_CODIGO)
      references PEDEP_DEPAR (PEDEP_CODIGO)
go

alter table GHCAM_CAMA
   add constraint FK_HAB_CAM foreign key (GHHAB_CODIGO)
      references GHHAB_HABIT (GHHAB_CODIGO)
go

alter table GHHOS_HOSPI
   add constraint FK_EPI_HOS foreign key (GHEPI_CODIGO)
      references GHEPI_EPISO (GHEPI_CODIGO)
go

alter table GHHOS_HOSPI
   add constraint FK_CAM_HOS foreign key (GHCAM_CODIGO)
      references GHCAM_CAMA (GHCAM_CODIGO)
go

alter table GHHOS_HOSPI
   add constraint FK_EMP_HOS foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHTRA_TRASL
   add constraint FK_HOS_TRA foreign key (GHHOS_CODIGO)
      references GHHOS_HOSPI (GHHOS_CODIGO)
go

alter table GHTRA_TRASL
   add constraint FK_CAM_TRA foreign key (GHCAM_CODIGO)
      references GHCAM_CAMA (GHCAM_CODIGO)
go

alter table GHTRA_TRASL
   add constraint FK_EMP_TRA foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table GHCIR_CIRUG
   add constraint FK_EPI_CIR foreign key (GHEPI_CODIGO)
      references GHEPI_EPISO (GHEPI_CODIGO)
go

alter table GHCIR_CIRUG
   add constraint FK_SAL_CIR foreign key (GHSAL_CODIGO)
      references GHSAL_SALA (GHSAL_CODIGO)
go

alter table GHCIR_CIRUG
   add constraint FK_EMP_CIR foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table FICUE_CUENT
   add constraint FK_EPI_CUE foreign key (GHEPI_CODIGO)
      references GHEPI_EPISO (GHEPI_CODIGO)
go

alter table FICAR_CARGO
   add constraint FK_CUE_CRG foreign key (FICUE_CODIGO)
      references FICUE_CUENT (FICUE_CODIGO)
go

alter table FICAR_CARGO
   add constraint FK_TAR_CRG foreign key (FITAR_CODIGO)
      references FITAR_TARIF (FITAR_CODIGO)
go

alter table FICOM_COMPR
   add constraint FK_CUE_COM foreign key (FICUE_CODIGO)
      references FICUE_CUENT (FICUE_CODIGO)
go

alter table FICOM_COMPR
   add constraint FK_EMP_COM foreign key (PEEMP_CODIGO)
      references PEEMP_EMPLE (PEEMP_CODIGO)
go

alter table FIPAG_PAGO
   add constraint FK_COM_PAG foreign key (FICOM_CODIGO)
      references FICOM_COMPR (FICOM_CODIGO)
go

alter table FIPAG_PAGO
   add constraint FK_FPA_PAG foreign key (FIFPA_CODIGO)
      references FIFPA_FORPA (FIFPA_CODIGO)
go

