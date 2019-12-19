
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/02/2018 11:12:26
-- Generated from EDMX file: E:\Project\ASPREE\Project\Aspree\Aspree.Data\AspreeContext.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Aspree];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Activity_Activity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_Activity];
GO
IF OBJECT_ID(N'[dbo].[FK_Activity_ActivityCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_ActivityCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_Activity_ActivityStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_ActivityStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_Activity_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_Activity_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [FK_Activity_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityCategory_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityCategory] DROP CONSTRAINT [FK_ActivityCategory_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityEntityType_Activity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityEntityType] DROP CONSTRAINT [FK_ActivityEntityType_Activity];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityEntityType_EntityType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityEntityType] DROP CONSTRAINT [FK_ActivityEntityType_EntityType];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityForm_Activity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityForm] DROP CONSTRAINT [FK_ActivityForm_Activity];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityForm_Forms]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityForm] DROP CONSTRAINT [FK_ActivityForm_Forms];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityFormRole_ActivityForm]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityFormRole] DROP CONSTRAINT [FK_ActivityFormRole_ActivityForm];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityFormRole_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityFormRole] DROP CONSTRAINT [FK_ActivityFormRole_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityRole_Activity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityRole] DROP CONSTRAINT [FK_ActivityRole_Activity];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityRole_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivityRole] DROP CONSTRAINT [FK_ActivityRole_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_Email_SmtpServer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Email] DROP CONSTRAINT [FK_Email_SmtpServer];
GO
IF OBJECT_ID(N'[dbo].[FK_EmailTemplate_PushEmailEvent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EmailTemplate] DROP CONSTRAINT [FK_EmailTemplate_PushEmailEvent];
GO
IF OBJECT_ID(N'[dbo].[FK_EmailTemplate_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EmailTemplate] DROP CONSTRAINT [FK_EmailTemplate_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_Entity_Entity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Entity] DROP CONSTRAINT [FK_Entity_Entity];
GO
IF OBJECT_ID(N'[dbo].[FK_Entity_EntityType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Entity] DROP CONSTRAINT [FK_Entity_EntityType];
GO
IF OBJECT_ID(N'[dbo].[FK_Entity_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Entity] DROP CONSTRAINT [FK_Entity_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_EntityDataVariable_Variable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EntityFormDataVariable] DROP CONSTRAINT [FK_EntityDataVariable_Variable];
GO
IF OBJECT_ID(N'[dbo].[FK_EntitySubType_EntityType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EntitySubType] DROP CONSTRAINT [FK_EntitySubType_EntityType];
GO
IF OBJECT_ID(N'[dbo].[FK_EntityType_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EntityType] DROP CONSTRAINT [FK_EntityType_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_EntityVariable_Entity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EntityVariable] DROP CONSTRAINT [FK_EntityVariable_Entity];
GO
IF OBJECT_ID(N'[dbo].[FK_EntityVariable_Variable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EntityVariable] DROP CONSTRAINT [FK_EntityVariable_Variable];
GO
IF OBJECT_ID(N'[dbo].[FK_Form_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Form] DROP CONSTRAINT [FK_Form_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_FormCategory_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormCategory] DROP CONSTRAINT [FK_FormCategory_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_FormDataEntry_Activity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormDataEntry] DROP CONSTRAINT [FK_FormDataEntry_Activity];
GO
IF OBJECT_ID(N'[dbo].[FK_FormDataEntry_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormDataEntry] DROP CONSTRAINT [FK_FormDataEntry_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_FormDataEntryVariable_FormDataEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormDataEntryVariable] DROP CONSTRAINT [FK_FormDataEntryVariable_FormDataEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_FormDataEntryVariable_MasterVariable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormDataEntryVariable] DROP CONSTRAINT [FK_FormDataEntryVariable_MasterVariable];
GO
IF OBJECT_ID(N'[dbo].[FK_FormEntityType_EntityType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormEntityType] DROP CONSTRAINT [FK_FormEntityType_EntityType];
GO
IF OBJECT_ID(N'[dbo].[FK_FormEntityType_Form]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormEntityType] DROP CONSTRAINT [FK_FormEntityType_Form];
GO
IF OBJECT_ID(N'[dbo].[FK_Forms_FormCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Form] DROP CONSTRAINT [FK_Forms_FormCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_Forms_FormStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Form] DROP CONSTRAINT [FK_Forms_FormStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_FormVariable_Forms]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormVariable] DROP CONSTRAINT [FK_FormVariable_Forms];
GO
IF OBJECT_ID(N'[dbo].[FK_FormVariable_MasterVariable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormVariable] DROP CONSTRAINT [FK_FormVariable_MasterVariable];
GO
IF OBJECT_ID(N'[dbo].[FK_FormVariableRole_FormVariable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormVariableRole] DROP CONSTRAINT [FK_FormVariableRole_FormVariable];
GO
IF OBJECT_ID(N'[dbo].[FK_FormVariableRole_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FormVariableRole] DROP CONSTRAINT [FK_FormVariableRole_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_LookupData_VariableType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LookupData] DROP CONSTRAINT [FK_LookupData_VariableType];
GO
IF OBJECT_ID(N'[dbo].[FK_LookupDataGroupOption_LookupDataOptionGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LookupDataGroupOption] DROP CONSTRAINT [FK_LookupDataGroupOption_LookupDataOptionGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_LookupDataOptionGroup_LookupData]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LookupDataOptionGroup] DROP CONSTRAINT [FK_LookupDataOptionGroup_LookupData];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterCity_MasterState]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[City] DROP CONSTRAINT [FK_MasterCity_MasterState];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterState_MasterContry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[State] DROP CONSTRAINT [FK_MasterState_MasterContry];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterVariableEntityType_EntityType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariableEntityType] DROP CONSTRAINT [FK_MasterVariableEntityType_EntityType];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterVariableEntityType_MasterVariable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariableEntityType] DROP CONSTRAINT [FK_MasterVariableEntityType_MasterVariable];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterVariableRole_MasterVariable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariableRole] DROP CONSTRAINT [FK_MasterVariableRole_MasterVariable];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterVariableRole_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariableRole] DROP CONSTRAINT [FK_MasterVariableRole_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_MasterVariableValue_MasterVariable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariableValue] DROP CONSTRAINT [FK_MasterVariableValue_MasterVariable];
GO
IF OBJECT_ID(N'[dbo].[FK_Project_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Project] DROP CONSTRAINT [FK_Project_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_Project_ProjectStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Project] DROP CONSTRAINT [FK_Project_ProjectStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_Project_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Project] DROP CONSTRAINT [FK_Project_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectCheckList_CheckList]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectCheckList] DROP CONSTRAINT [FK_ProjectCheckList_CheckList];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectCheckList_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectCheckList] DROP CONSTRAINT [FK_ProjectCheckList_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectEntityType_EntityType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectEntityType] DROP CONSTRAINT [FK_ProjectEntityType_EntityType];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectEntityType_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectEntityType] DROP CONSTRAINT [FK_ProjectEntityType_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectParticipant_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectParticipant] DROP CONSTRAINT [FK_ProjectParticipant_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectRole_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectRole] DROP CONSTRAINT [FK_ProjectRole_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectRole_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectRole] DROP CONSTRAINT [FK_ProjectRole_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectSite_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectSite] DROP CONSTRAINT [FK_ProjectSite_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectSite_Site]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectSite] DROP CONSTRAINT [FK_ProjectSite_Site];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectStaffMember_Project]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectStaffMember] DROP CONSTRAINT [FK_ProjectStaffMember_Project];
GO
IF OBJECT_ID(N'[dbo].[FK_ProjectStaffMember_UserLogin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProjectStaffMember] DROP CONSTRAINT [FK_ProjectStaffMember_UserLogin];
GO
IF OBJECT_ID(N'[dbo].[FK_ReviewerFeedback_Form]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ReviewerFeedback] DROP CONSTRAINT [FK_ReviewerFeedback_Form];
GO
IF OBJECT_ID(N'[dbo].[FK_Role_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Role] DROP CONSTRAINT [FK_Role_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_RolePrivilege_Privilege]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RolePrivilege] DROP CONSTRAINT [FK_RolePrivilege_Privilege];
GO
IF OBJECT_ID(N'[dbo].[FK_RolePrivilege_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RolePrivilege] DROP CONSTRAINT [FK_RolePrivilege_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_Site_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Site] DROP CONSTRAINT [FK_Site_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_UserLogin_AuthTypeMaster]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserLogin] DROP CONSTRAINT [FK_UserLogin_AuthTypeMaster];
GO
IF OBJECT_ID(N'[dbo].[FK_UserLogin_SecurityQuestion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserLogin] DROP CONSTRAINT [FK_UserLogin_SecurityQuestion];
GO
IF OBJECT_ID(N'[dbo].[FK_UserLogin_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserLogin] DROP CONSTRAINT [FK_UserLogin_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_UserRole_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_UserRole_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_UserRole_UserLogin]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserRole] DROP CONSTRAINT [FK_UserRole_UserLogin];
GO
IF OBJECT_ID(N'[dbo].[FK_ValidationRule_RegExRule]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ValidationRule] DROP CONSTRAINT [FK_ValidationRule_RegExRule];
GO
IF OBJECT_ID(N'[dbo].[FK_Variable_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Variable] DROP CONSTRAINT [FK_Variable_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_Variable_VariableCategory]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Variable] DROP CONSTRAINT [FK_Variable_VariableCategory];
GO
IF OBJECT_ID(N'[dbo].[FK_Variable_VariableType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Variable] DROP CONSTRAINT [FK_Variable_VariableType];
GO
IF OBJECT_ID(N'[dbo].[FK_VariableApprovalLog_Variable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariableApprovalLog] DROP CONSTRAINT [FK_VariableApprovalLog_Variable];
GO
IF OBJECT_ID(N'[dbo].[FK_VariableCategory_Tenant]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariableCategory] DROP CONSTRAINT [FK_VariableCategory_Tenant];
GO
IF OBJECT_ID(N'[dbo].[FK_VariableValidationRule_ValidationRule]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariableValidationRule] DROP CONSTRAINT [FK_VariableValidationRule_ValidationRule];
GO
IF OBJECT_ID(N'[dbo].[FK_VariableValidationRule_Variable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VariableValidationRule] DROP CONSTRAINT [FK_VariableValidationRule_Variable];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Activity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Activity];
GO
IF OBJECT_ID(N'[dbo].[ActivityCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityCategory];
GO
IF OBJECT_ID(N'[dbo].[ActivityEntityType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityEntityType];
GO
IF OBJECT_ID(N'[dbo].[ActivityForm]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityForm];
GO
IF OBJECT_ID(N'[dbo].[ActivityFormRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityFormRole];
GO
IF OBJECT_ID(N'[dbo].[ActivityRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityRole];
GO
IF OBJECT_ID(N'[dbo].[ActivityStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityStatus];
GO
IF OBJECT_ID(N'[dbo].[AuthTypeMaster]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AuthTypeMaster];
GO
IF OBJECT_ID(N'[dbo].[CheckList]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CheckList];
GO
IF OBJECT_ID(N'[dbo].[City]', 'U') IS NOT NULL
    DROP TABLE [dbo].[City];
GO
IF OBJECT_ID(N'[dbo].[ContactType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContactType];
GO
IF OBJECT_ID(N'[dbo].[Country]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Country];
GO
IF OBJECT_ID(N'[dbo].[Email]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Email];
GO
IF OBJECT_ID(N'[dbo].[EmailTemplate]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmailTemplate];
GO
IF OBJECT_ID(N'[dbo].[Entity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Entity];
GO
IF OBJECT_ID(N'[dbo].[EntityFormDataVariable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EntityFormDataVariable];
GO
IF OBJECT_ID(N'[dbo].[EntitySubType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EntitySubType];
GO
IF OBJECT_ID(N'[dbo].[EntityType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EntityType];
GO
IF OBJECT_ID(N'[dbo].[EntityVariable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EntityVariable];
GO
IF OBJECT_ID(N'[dbo].[ExternalLink]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExternalLink];
GO
IF OBJECT_ID(N'[dbo].[Form]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Form];
GO
IF OBJECT_ID(N'[dbo].[FormCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FormCategory];
GO
IF OBJECT_ID(N'[dbo].[FormDataEntry]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FormDataEntry];
GO
IF OBJECT_ID(N'[dbo].[FormDataEntryVariable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FormDataEntryVariable];
GO
IF OBJECT_ID(N'[dbo].[FormEntityType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FormEntityType];
GO
IF OBJECT_ID(N'[dbo].[FormStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FormStatus];
GO
IF OBJECT_ID(N'[dbo].[FormVariable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FormVariable];
GO
IF OBJECT_ID(N'[dbo].[FormVariableRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FormVariableRole];
GO
IF OBJECT_ID(N'[dbo].[LookupData]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LookupData];
GO
IF OBJECT_ID(N'[dbo].[LookupDataGroupOption]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LookupDataGroupOption];
GO
IF OBJECT_ID(N'[dbo].[LookupDataOptionGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LookupDataOptionGroup];
GO
IF OBJECT_ID(N'[dbo].[Privilege]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Privilege];
GO
IF OBJECT_ID(N'[dbo].[Project]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Project];
GO
IF OBJECT_ID(N'[dbo].[ProjectCheckList]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProjectCheckList];
GO
IF OBJECT_ID(N'[dbo].[ProjectEntityType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProjectEntityType];
GO
IF OBJECT_ID(N'[dbo].[ProjectParticipant]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProjectParticipant];
GO
IF OBJECT_ID(N'[dbo].[ProjectRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProjectRole];
GO
IF OBJECT_ID(N'[dbo].[ProjectSite]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProjectSite];
GO
IF OBJECT_ID(N'[dbo].[ProjectStaffMember]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProjectStaffMember];
GO
IF OBJECT_ID(N'[dbo].[ProjectStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProjectStatus];
GO
IF OBJECT_ID(N'[dbo].[PushEmailEvent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PushEmailEvent];
GO
IF OBJECT_ID(N'[dbo].[RegExRule]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegExRule];
GO
IF OBJECT_ID(N'[dbo].[ReviewerFeedback]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ReviewerFeedback];
GO
IF OBJECT_ID(N'[dbo].[Role]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Role];
GO
IF OBJECT_ID(N'[dbo].[RolePrivilege]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RolePrivilege];
GO
IF OBJECT_ID(N'[dbo].[SecurityQuestion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SecurityQuestion];
GO
IF OBJECT_ID(N'[dbo].[Site]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Site];
GO
IF OBJECT_ID(N'[dbo].[SmtpServer]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SmtpServer];
GO
IF OBJECT_ID(N'[dbo].[State]', 'U') IS NOT NULL
    DROP TABLE [dbo].[State];
GO
IF OBJECT_ID(N'[dbo].[Status]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Status];
GO
IF OBJECT_ID(N'[dbo].[Tenant]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tenant];
GO
IF OBJECT_ID(N'[dbo].[User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User];
GO
IF OBJECT_ID(N'[dbo].[UserLogin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserLogin];
GO
IF OBJECT_ID(N'[dbo].[UserRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserRole];
GO
IF OBJECT_ID(N'[dbo].[ValidationRule]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ValidationRule];
GO
IF OBJECT_ID(N'[dbo].[Variable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Variable];
GO
IF OBJECT_ID(N'[dbo].[VariableApprovalLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VariableApprovalLog];
GO
IF OBJECT_ID(N'[dbo].[VariableCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VariableCategory];
GO
IF OBJECT_ID(N'[dbo].[VariableEntityType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VariableEntityType];
GO
IF OBJECT_ID(N'[dbo].[VariableRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VariableRole];
GO
IF OBJECT_ID(N'[dbo].[VariableType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VariableType];
GO
IF OBJECT_ID(N'[dbo].[VariableValidationRule]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VariableValidationRule];
GO
IF OBJECT_ID(N'[dbo].[VariableValue]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VariableValue];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Activities'
CREATE TABLE [dbo].[Activities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ActivityName] nvarchar(250)  NULL,
    [RepeatationType] int  NULL,
    [RepeatationCount] int  NULL,
    [RepeatationOffset] int  NULL,
    [DependentActivityId] int  NULL,
    [ActivityCategoryId] int  NULL,
    [StartDate] datetime  NULL,
    [EndDate] datetime  NULL,
    [ScheduleType] int  NOT NULL,
    [ActivityStatusId] int  NOT NULL,
    [ProjectId] int  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'ActivityCategories'
CREATE TABLE [dbo].[ActivityCategories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CategoryName] nvarchar(50)  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'ActivityEntityTypes'
CREATE TABLE [dbo].[ActivityEntityTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ActivityId] int  NOT NULL,
    [EntityTypeId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ActivityForms'
CREATE TABLE [dbo].[ActivityForms] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FormId] int  NOT NULL,
    [ActivityId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ActivityFormRoles'
CREATE TABLE [dbo].[ActivityFormRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ActivityFormId] int  NOT NULL,
    [RoleId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ActivityRoles'
CREATE TABLE [dbo].[ActivityRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ActivityId] int  NOT NULL,
    [RoleId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ActivityStatus'
CREATE TABLE [dbo].[ActivityStatus] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Status] varchar(50)  NULL,
    [IsActive] bit  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AuthTypeMasters'
CREATE TABLE [dbo].[AuthTypeMasters] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AuthType] nvarchar(150)  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'CheckLists'
CREATE TABLE [dbo].[CheckLists] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(100)  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Cities'
CREATE TABLE [dbo].[Cities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [StatedId] int  NOT NULL,
    [Abbr] varchar(50)  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ContactTypes'
CREATE TABLE [dbo].[ContactTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] varchar(50)  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Countries'
CREATE TABLE [dbo].[Countries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [Abbr] varchar(50)  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Emails'
CREATE TABLE [dbo].[Emails] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [To] nvarchar(250)  NULL,
    [From] nvarchar(100)  NULL,
    [Subject] nvarchar(250)  NULL,
    [Body] nvarchar(max)  NULL,
    [IsSend] bit  NOT NULL,
    [FailedAttempt] int  NULL,
    [IsImmediate] bit  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [SentDate] datetime  NULL,
    [SmtpServerId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'EmailTemplates'
CREATE TABLE [dbo].[EmailTemplates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PushEmailEventId] int  NOT NULL,
    [Subject] nvarchar(100)  NOT NULL,
    [MailBody] nvarchar(max)  NULL,
    [EmailKeywords] nvarchar(max)  NULL,
    [IsActive] bit  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'Entities'
CREATE TABLE [dbo].[Entities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EntityTypeId] int  NOT NULL,
    [EntitySubTypeId] int  NULL,
    [Name] nvarchar(50)  NULL,
    [ParentEntityId] int  NULL,
    [Status] int  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'EntityFormDataVariables'
CREATE TABLE [dbo].[EntityFormDataVariables] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EntityId] int  NOT NULL,
    [VariableId] int  NOT NULL,
    [SelectedValues] nvarchar(max)  NOT NULL,
    [EntityName] nvarchar(50)  NOT NULL,
    [EntityTypeId] int  NOT NULL,
    [EntitySubTypeId] int  NULL,
    [Json] nvarchar(max)  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'EntitySubTypes'
CREATE TABLE [dbo].[EntitySubTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [EntityTypeId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'EntityTypes'
CREATE TABLE [dbo].[EntityTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'EntityVariables'
CREATE TABLE [dbo].[EntityVariables] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EntityId] int  NOT NULL,
    [VariableId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ExternalLinks'
CREATE TABLE [dbo].[ExternalLinks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [Url] nvarchar(500)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ExpiredDate] datetime  NOT NULL,
    [UserGuid] uniqueidentifier  NULL
);
GO

-- Creating table 'Forms'
CREATE TABLE [dbo].[Forms] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FormTitle] nvarchar(50)  NOT NULL,
    [IsPublished] bit  NOT NULL,
    [ApprovedBy] int  NULL,
    [ApprovedDate] datetime  NULL,
    [IsTemplate] bit  NOT NULL,
    [FormCategoryId] int  NULL,
    [ProjectId] int  NULL,
    [FormState] int  NOT NULL,
    [FormStatusId] int  NOT NULL,
    [Version] int  NULL,
    [PreviousVersion] int  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'FormCategories'
CREATE TABLE [dbo].[FormCategories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CategoryName] nvarchar(50)  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'FormDataEntries'
CREATE TABLE [dbo].[FormDataEntries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ActivityId] int  NOT NULL,
    [ProjectId] int  NOT NULL,
    [EntityId] int  NOT NULL,
    [SubjectId] int  NULL,
    [Status] int  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'FormDataEntryVariables'
CREATE TABLE [dbo].[FormDataEntryVariables] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [VariableId] int  NOT NULL,
    [SelectedValues] nvarchar(max)  NULL,
    [SelectedValues_int] int  NULL,
    [SelectedValues_float] float  NULL,
    [FormDataEntryId] int  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'FormEntityTypes'
CREATE TABLE [dbo].[FormEntityTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FormId] int  NOT NULL,
    [EntityTypeId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'FormStatus'
CREATE TABLE [dbo].[FormStatus] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Status] varchar(50)  NULL,
    [IsActive] bit  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'FormVariables'
CREATE TABLE [dbo].[FormVariables] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FormId] int  NOT NULL,
    [VariableId] int  NOT NULL,
    [IsRequired] bit  NOT NULL,
    [HelpText] nvarchar(250)  NULL,
    [MinRange] int  NULL,
    [MaxRange] int  NULL,
    [RegEx] int  NULL,
    [ValidationRuleType] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [DependentVariableId] int  NULL,
    [ResponseOption] nvarchar(250)  NULL,
    [ValidationMessage] nvarchar(250)  NULL
);
GO

-- Creating table 'FormVariableRoles'
CREATE TABLE [dbo].[FormVariableRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FormVariableId] int  NOT NULL,
    [RoleId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'LookupDatas'
CREATE TABLE [dbo].[LookupDatas] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(50)  NOT NULL,
    [Status] int  NOT NULL,
    [VariableTypeId] int  NOT NULL,
    [LookupDataType] int  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NULL
);
GO

-- Creating table 'LookupDataGroupOptions'
CREATE TABLE [dbo].[LookupDataGroupOptions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Text] nvarchar(250)  NOT NULL,
    [Value] nvarchar(50)  NOT NULL,
    [LookupDataOptionGroupId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'LookupDataOptionGroups'
CREATE TABLE [dbo].[LookupDataOptionGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Label] nvarchar(250)  NOT NULL,
    [Value] nvarchar(50)  NOT NULL,
    [LookupDataId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Privileges'
CREATE TABLE [dbo].[Privileges] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NULL,
    [IsProjectAdmin] bit  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Projects'
CREATE TABLE [dbo].[Projects] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProjectName] nvarchar(50)  NOT NULL,
    [ProjectStatusId] int  NOT NULL,
    [State] int  NOT NULL,
    [Version] int  NOT NULL,
    [PreviousProjectId] int  NULL,
    [ProjectUrl] nvarchar(250)  NULL,
    [CheckListID] int  NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'ProjectCheckLists'
CREATE TABLE [dbo].[ProjectCheckLists] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProjectId] int  NOT NULL,
    [CheckListId] int  NOT NULL,
    [CommonField] nvarchar(100)  NULL,
    [Status] bit  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ProjectEntityTypes'
CREATE TABLE [dbo].[ProjectEntityTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProjectId] int  NOT NULL,
    [EntityTypeId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ProjectParticipants'
CREATE TABLE [dbo].[ProjectParticipants] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProjectId] int  NOT NULL,
    [EntityId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ProjectRoles'
CREATE TABLE [dbo].[ProjectRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProjectId] int  NOT NULL,
    [RoleId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ProjectSites'
CREATE TABLE [dbo].[ProjectSites] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProjectId] int  NOT NULL,
    [SiteId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ProjectStaffMembers'
CREATE TABLE [dbo].[ProjectStaffMembers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProjectId] int  NOT NULL,
    [UserId] int  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ProjectStatus'
CREATE TABLE [dbo].[ProjectStatus] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Status] nvarchar(50)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'PushEmailEvents'
CREATE TABLE [dbo].[PushEmailEvents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EventName] nvarchar(max)  NULL,
    [IsEmailEvent] bit  NULL,
    [DisplayName] nvarchar(max)  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RegExRules'
CREATE TABLE [dbo].[RegExRules] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RegExName] nvarchar(50)  NOT NULL,
    [RegEx] nvarchar(150)  NULL,
    [Description] nvarchar(200)  NOT NULL,
    [Guid] uniqueidentifier  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL
);
GO

-- Creating table 'ReviewerFeedbacks'
CREATE TABLE [dbo].[ReviewerFeedbacks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserID] int  NOT NULL,
    [FormId] int  NOT NULL,
    [Comments] nvarchar(500)  NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Roles'
CREATE TABLE [dbo].[Roles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [IsSystemRole] bit  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [Status] int  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'RolePrivileges'
CREATE TABLE [dbo].[RolePrivileges] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleId] int  NOT NULL,
    [PrivilegeId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'SecurityQuestions'
CREATE TABLE [dbo].[SecurityQuestions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Question] nvarchar(100)  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Sites'
CREATE TABLE [dbo].[Sites] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CountyId] int  NULL,
    [StateId] int  NULL,
    [CityId] int  NULL,
    [AddressLine1] nvarchar(200)  NULL,
    [AddressLine2] nvarchar(200)  NULL,
    [Suburb] nvarchar(200)  NULL,
    [PostCode] nvarchar(50)  NULL,
    [GPSLocations] nvarchar(150)  NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'SmtpServers'
CREATE TABLE [dbo].[SmtpServers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [FromEmail] nvarchar(50)  NOT NULL,
    [Password] nvarchar(50)  NOT NULL,
    [Port] int  NOT NULL,
    [MailServer] nvarchar(50)  NOT NULL,
    [Status] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'States'
CREATE TABLE [dbo].[States] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [CountryId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Status'
CREATE TABLE [dbo].[Status] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Status1] nvarchar(50)  NULL,
    [IsActive] bit  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Tenants'
CREATE TABLE [dbo].[Tenants] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [Email] nvarchar(50)  NOT NULL,
    [CompanyName] nvarchar(50)  NOT NULL,
    [FirstName] nvarchar(50)  NOT NULL,
    [LastName] nvarchar(50)  NOT NULL,
    [Status] int  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(50)  NOT NULL,
    [LastName] nvarchar(50)  NOT NULL,
    [Email] nvarchar(50)  NOT NULL,
    [Mobile] varchar(20)  NULL,
    [Address] varchar(250)  NULL,
    [SkypeId] varchar(100)  NULL
);
GO

-- Creating table 'UserLogins'
CREATE TABLE [dbo].[UserLogins] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(50)  NOT NULL,
    [LastName] nvarchar(50)  NOT NULL,
    [Email] nvarchar(50)  NOT NULL,
    [Mobile] varchar(20)  NULL,
    [Address] varchar(250)  NULL,
    [Password] nvarchar(50)  NULL,
    [Salt] nvarchar(50)  NULL,
    [SecurityQuestionId] int  NULL,
    [Answer] nvarchar(50)  NULL,
    [AccessToken] nvarchar(50)  NULL,
    [TenantId] int  NOT NULL,
    [AuthTypeId] int  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [Status] int  NULL
);
GO

-- Creating table 'UserRoles'
CREATE TABLE [dbo].[UserRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [RoleId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'ValidationRules'
CREATE TABLE [dbo].[ValidationRules] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RuleType] nvarchar(50)  NOT NULL,
    [MinRange] float  NULL,
    [MaxRange] float  NULL,
    [RegExId] int  NULL,
    [ErrorMessage] nvarchar(250)  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Variables'
CREATE TABLE [dbo].[Variables] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [VariableName] nvarchar(50)  NOT NULL,
    [VariableLabel] nvarchar(50)  NOT NULL,
    [Question] nvarchar(max)  NULL,
    [Values] nvarchar(max)  NULL,
    [ValueDescription] nvarchar(250)  NULL,
    [HelpText] nvarchar(250)  NULL,
    [VariableTypeId] int  NOT NULL,
    [ValidationMessage] nvarchar(250)  NULL,
    [RequiredMessage] nvarchar(250)  NULL,
    [MinRange] float  NULL,
    [MaxRange] float  NULL,
    [RegEx] varchar(250)  NULL,
    [IsSoftRange] bit  NULL,
    [ValidationRuleId] int  NULL,
    [DependentVariableId] int  NULL,
    [IsRequired] bit  NOT NULL,
    [CanCollectMultiple] bit  NOT NULL,
    [VariableCategoryId] int  NULL,
    [IsApproved] bit  NOT NULL,
    [Comment] nvarchar(250)  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL,
    [VariableValueDescription] nvarchar(max)  NULL
);
GO

-- Creating table 'VariableCategories'
CREATE TABLE [dbo].[VariableCategories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CategoryName] nvarchar(50)  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NULL,
    [ModifiedDate] datetime  NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- Creating table 'VariableEntityTypes'
CREATE TABLE [dbo].[VariableEntityTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [VariableId] int  NOT NULL,
    [EntityTypeId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'VariableRoles'
CREATE TABLE [dbo].[VariableRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [VariableId] int  NOT NULL,
    [RoleId] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'VariableTypes'
CREATE TABLE [dbo].[VariableTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(50)  NOT NULL,
    [Status] int  NOT NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'VariableValues'
CREATE TABLE [dbo].[VariableValues] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [VariableId] int  NOT NULL,
    [Text] nvarchar(50)  NOT NULL,
    [Value] nvarchar(50)  NOT NULL,
    [ResponseVariableId] int  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'VariableValidationRules'
CREATE TABLE [dbo].[VariableValidationRules] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [VariableId] int  NOT NULL,
    [ValidationId] int  NULL,
    [ValidationMessage] nvarchar(200)  NOT NULL,
    [RegEx] nvarchar(200)  NULL,
    [Min] int  NULL,
    [Max] int  NULL,
    [LimitType] nvarchar(50)  NULL,
    [Guid] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'VariableApprovalLogs'
CREATE TABLE [dbo].[VariableApprovalLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [VariableId] int  NOT NULL,
    [CommentText] nvarchar(200)  NULL,
    [CreatedBy] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [DeactivatedBy] int  NULL,
    [DateDeactivated] datetime  NULL,
    [Guid] uniqueidentifier  NOT NULL,
    [TenantId] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [PK_Activities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActivityCategories'
ALTER TABLE [dbo].[ActivityCategories]
ADD CONSTRAINT [PK_ActivityCategories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActivityEntityTypes'
ALTER TABLE [dbo].[ActivityEntityTypes]
ADD CONSTRAINT [PK_ActivityEntityTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActivityForms'
ALTER TABLE [dbo].[ActivityForms]
ADD CONSTRAINT [PK_ActivityForms]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActivityFormRoles'
ALTER TABLE [dbo].[ActivityFormRoles]
ADD CONSTRAINT [PK_ActivityFormRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActivityRoles'
ALTER TABLE [dbo].[ActivityRoles]
ADD CONSTRAINT [PK_ActivityRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActivityStatus'
ALTER TABLE [dbo].[ActivityStatus]
ADD CONSTRAINT [PK_ActivityStatus]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AuthTypeMasters'
ALTER TABLE [dbo].[AuthTypeMasters]
ADD CONSTRAINT [PK_AuthTypeMasters]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CheckLists'
ALTER TABLE [dbo].[CheckLists]
ADD CONSTRAINT [PK_CheckLists]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Cities'
ALTER TABLE [dbo].[Cities]
ADD CONSTRAINT [PK_Cities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContactTypes'
ALTER TABLE [dbo].[ContactTypes]
ADD CONSTRAINT [PK_ContactTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Countries'
ALTER TABLE [dbo].[Countries]
ADD CONSTRAINT [PK_Countries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Emails'
ALTER TABLE [dbo].[Emails]
ADD CONSTRAINT [PK_Emails]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EmailTemplates'
ALTER TABLE [dbo].[EmailTemplates]
ADD CONSTRAINT [PK_EmailTemplates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Entities'
ALTER TABLE [dbo].[Entities]
ADD CONSTRAINT [PK_Entities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EntityFormDataVariables'
ALTER TABLE [dbo].[EntityFormDataVariables]
ADD CONSTRAINT [PK_EntityFormDataVariables]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EntitySubTypes'
ALTER TABLE [dbo].[EntitySubTypes]
ADD CONSTRAINT [PK_EntitySubTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EntityTypes'
ALTER TABLE [dbo].[EntityTypes]
ADD CONSTRAINT [PK_EntityTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EntityVariables'
ALTER TABLE [dbo].[EntityVariables]
ADD CONSTRAINT [PK_EntityVariables]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExternalLinks'
ALTER TABLE [dbo].[ExternalLinks]
ADD CONSTRAINT [PK_ExternalLinks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Forms'
ALTER TABLE [dbo].[Forms]
ADD CONSTRAINT [PK_Forms]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FormCategories'
ALTER TABLE [dbo].[FormCategories]
ADD CONSTRAINT [PK_FormCategories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FormDataEntries'
ALTER TABLE [dbo].[FormDataEntries]
ADD CONSTRAINT [PK_FormDataEntries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FormDataEntryVariables'
ALTER TABLE [dbo].[FormDataEntryVariables]
ADD CONSTRAINT [PK_FormDataEntryVariables]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FormEntityTypes'
ALTER TABLE [dbo].[FormEntityTypes]
ADD CONSTRAINT [PK_FormEntityTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FormStatus'
ALTER TABLE [dbo].[FormStatus]
ADD CONSTRAINT [PK_FormStatus]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FormVariables'
ALTER TABLE [dbo].[FormVariables]
ADD CONSTRAINT [PK_FormVariables]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FormVariableRoles'
ALTER TABLE [dbo].[FormVariableRoles]
ADD CONSTRAINT [PK_FormVariableRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LookupDatas'
ALTER TABLE [dbo].[LookupDatas]
ADD CONSTRAINT [PK_LookupDatas]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LookupDataGroupOptions'
ALTER TABLE [dbo].[LookupDataGroupOptions]
ADD CONSTRAINT [PK_LookupDataGroupOptions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LookupDataOptionGroups'
ALTER TABLE [dbo].[LookupDataOptionGroups]
ADD CONSTRAINT [PK_LookupDataOptionGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Privileges'
ALTER TABLE [dbo].[Privileges]
ADD CONSTRAINT [PK_Privileges]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Projects'
ALTER TABLE [dbo].[Projects]
ADD CONSTRAINT [PK_Projects]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProjectCheckLists'
ALTER TABLE [dbo].[ProjectCheckLists]
ADD CONSTRAINT [PK_ProjectCheckLists]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProjectEntityTypes'
ALTER TABLE [dbo].[ProjectEntityTypes]
ADD CONSTRAINT [PK_ProjectEntityTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProjectParticipants'
ALTER TABLE [dbo].[ProjectParticipants]
ADD CONSTRAINT [PK_ProjectParticipants]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProjectRoles'
ALTER TABLE [dbo].[ProjectRoles]
ADD CONSTRAINT [PK_ProjectRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProjectSites'
ALTER TABLE [dbo].[ProjectSites]
ADD CONSTRAINT [PK_ProjectSites]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProjectStaffMembers'
ALTER TABLE [dbo].[ProjectStaffMembers]
ADD CONSTRAINT [PK_ProjectStaffMembers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProjectStatus'
ALTER TABLE [dbo].[ProjectStatus]
ADD CONSTRAINT [PK_ProjectStatus]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PushEmailEvents'
ALTER TABLE [dbo].[PushEmailEvents]
ADD CONSTRAINT [PK_PushEmailEvents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RegExRules'
ALTER TABLE [dbo].[RegExRules]
ADD CONSTRAINT [PK_RegExRules]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ReviewerFeedbacks'
ALTER TABLE [dbo].[ReviewerFeedbacks]
ADD CONSTRAINT [PK_ReviewerFeedbacks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [PK_Roles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RolePrivileges'
ALTER TABLE [dbo].[RolePrivileges]
ADD CONSTRAINT [PK_RolePrivileges]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SecurityQuestions'
ALTER TABLE [dbo].[SecurityQuestions]
ADD CONSTRAINT [PK_SecurityQuestions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Sites'
ALTER TABLE [dbo].[Sites]
ADD CONSTRAINT [PK_Sites]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SmtpServers'
ALTER TABLE [dbo].[SmtpServers]
ADD CONSTRAINT [PK_SmtpServers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'States'
ALTER TABLE [dbo].[States]
ADD CONSTRAINT [PK_States]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Status'
ALTER TABLE [dbo].[Status]
ADD CONSTRAINT [PK_Status]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tenants'
ALTER TABLE [dbo].[Tenants]
ADD CONSTRAINT [PK_Tenants]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserLogins'
ALTER TABLE [dbo].[UserLogins]
ADD CONSTRAINT [PK_UserLogins]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [PK_UserRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ValidationRules'
ALTER TABLE [dbo].[ValidationRules]
ADD CONSTRAINT [PK_ValidationRules]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Variables'
ALTER TABLE [dbo].[Variables]
ADD CONSTRAINT [PK_Variables]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VariableCategories'
ALTER TABLE [dbo].[VariableCategories]
ADD CONSTRAINT [PK_VariableCategories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VariableEntityTypes'
ALTER TABLE [dbo].[VariableEntityTypes]
ADD CONSTRAINT [PK_VariableEntityTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VariableRoles'
ALTER TABLE [dbo].[VariableRoles]
ADD CONSTRAINT [PK_VariableRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VariableTypes'
ALTER TABLE [dbo].[VariableTypes]
ADD CONSTRAINT [PK_VariableTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VariableValues'
ALTER TABLE [dbo].[VariableValues]
ADD CONSTRAINT [PK_VariableValues]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VariableValidationRules'
ALTER TABLE [dbo].[VariableValidationRules]
ADD CONSTRAINT [PK_VariableValidationRules]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VariableApprovalLogs'
ALTER TABLE [dbo].[VariableApprovalLogs]
ADD CONSTRAINT [PK_VariableApprovalLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [DependentActivityId] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [FK_Activity_Activity]
    FOREIGN KEY ([DependentActivityId])
    REFERENCES [dbo].[Activities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Activity_Activity'
CREATE INDEX [IX_FK_Activity_Activity]
ON [dbo].[Activities]
    ([DependentActivityId]);
GO

-- Creating foreign key on [ActivityCategoryId] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [FK_Activity_ActivityCategory]
    FOREIGN KEY ([ActivityCategoryId])
    REFERENCES [dbo].[ActivityCategories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Activity_ActivityCategory'
CREATE INDEX [IX_FK_Activity_ActivityCategory]
ON [dbo].[Activities]
    ([ActivityCategoryId]);
GO

-- Creating foreign key on [ActivityStatusId] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [FK_Activity_ActivityStatus]
    FOREIGN KEY ([ActivityStatusId])
    REFERENCES [dbo].[ActivityStatus]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Activity_ActivityStatus'
CREATE INDEX [IX_FK_Activity_ActivityStatus]
ON [dbo].[Activities]
    ([ActivityStatusId]);
GO

-- Creating foreign key on [ProjectId] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [FK_Activity_Project]
    FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Activity_Project'
CREATE INDEX [IX_FK_Activity_Project]
ON [dbo].[Activities]
    ([ProjectId]);
GO

-- Creating foreign key on [TenantId] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [FK_Activity_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Activity_Tenant'
CREATE INDEX [IX_FK_Activity_Tenant]
ON [dbo].[Activities]
    ([TenantId]);
GO

-- Creating foreign key on [ActivityId] in table 'ActivityEntityTypes'
ALTER TABLE [dbo].[ActivityEntityTypes]
ADD CONSTRAINT [FK_ActivityEntityType_Activity]
    FOREIGN KEY ([ActivityId])
    REFERENCES [dbo].[Activities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityEntityType_Activity'
CREATE INDEX [IX_FK_ActivityEntityType_Activity]
ON [dbo].[ActivityEntityTypes]
    ([ActivityId]);
GO

-- Creating foreign key on [ActivityId] in table 'ActivityForms'
ALTER TABLE [dbo].[ActivityForms]
ADD CONSTRAINT [FK_ActivityForm_Activity]
    FOREIGN KEY ([ActivityId])
    REFERENCES [dbo].[Activities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityForm_Activity'
CREATE INDEX [IX_FK_ActivityForm_Activity]
ON [dbo].[ActivityForms]
    ([ActivityId]);
GO

-- Creating foreign key on [ActivityId] in table 'ActivityRoles'
ALTER TABLE [dbo].[ActivityRoles]
ADD CONSTRAINT [FK_ActivityRole_Activity]
    FOREIGN KEY ([ActivityId])
    REFERENCES [dbo].[Activities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityRole_Activity'
CREATE INDEX [IX_FK_ActivityRole_Activity]
ON [dbo].[ActivityRoles]
    ([ActivityId]);
GO

-- Creating foreign key on [ActivityId] in table 'FormDataEntries'
ALTER TABLE [dbo].[FormDataEntries]
ADD CONSTRAINT [FK_FormDataEntry_Activity]
    FOREIGN KEY ([ActivityId])
    REFERENCES [dbo].[Activities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormDataEntry_Activity'
CREATE INDEX [IX_FK_FormDataEntry_Activity]
ON [dbo].[FormDataEntries]
    ([ActivityId]);
GO

-- Creating foreign key on [TenantId] in table 'ActivityCategories'
ALTER TABLE [dbo].[ActivityCategories]
ADD CONSTRAINT [FK_ActivityCategory_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityCategory_Tenant'
CREATE INDEX [IX_FK_ActivityCategory_Tenant]
ON [dbo].[ActivityCategories]
    ([TenantId]);
GO

-- Creating foreign key on [EntityTypeId] in table 'ActivityEntityTypes'
ALTER TABLE [dbo].[ActivityEntityTypes]
ADD CONSTRAINT [FK_ActivityEntityType_EntityType]
    FOREIGN KEY ([EntityTypeId])
    REFERENCES [dbo].[EntityTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityEntityType_EntityType'
CREATE INDEX [IX_FK_ActivityEntityType_EntityType]
ON [dbo].[ActivityEntityTypes]
    ([EntityTypeId]);
GO

-- Creating foreign key on [FormId] in table 'ActivityForms'
ALTER TABLE [dbo].[ActivityForms]
ADD CONSTRAINT [FK_ActivityForm_Forms]
    FOREIGN KEY ([FormId])
    REFERENCES [dbo].[Forms]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityForm_Forms'
CREATE INDEX [IX_FK_ActivityForm_Forms]
ON [dbo].[ActivityForms]
    ([FormId]);
GO

-- Creating foreign key on [ActivityFormId] in table 'ActivityFormRoles'
ALTER TABLE [dbo].[ActivityFormRoles]
ADD CONSTRAINT [FK_ActivityFormRole_ActivityForm]
    FOREIGN KEY ([ActivityFormId])
    REFERENCES [dbo].[ActivityForms]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityFormRole_ActivityForm'
CREATE INDEX [IX_FK_ActivityFormRole_ActivityForm]
ON [dbo].[ActivityFormRoles]
    ([ActivityFormId]);
GO

-- Creating foreign key on [RoleId] in table 'ActivityFormRoles'
ALTER TABLE [dbo].[ActivityFormRoles]
ADD CONSTRAINT [FK_ActivityFormRole_Role]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityFormRole_Role'
CREATE INDEX [IX_FK_ActivityFormRole_Role]
ON [dbo].[ActivityFormRoles]
    ([RoleId]);
GO

-- Creating foreign key on [RoleId] in table 'ActivityRoles'
ALTER TABLE [dbo].[ActivityRoles]
ADD CONSTRAINT [FK_ActivityRole_Role]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityRole_Role'
CREATE INDEX [IX_FK_ActivityRole_Role]
ON [dbo].[ActivityRoles]
    ([RoleId]);
GO

-- Creating foreign key on [AuthTypeId] in table 'UserLogins'
ALTER TABLE [dbo].[UserLogins]
ADD CONSTRAINT [FK_UserLogin_AuthTypeMaster]
    FOREIGN KEY ([AuthTypeId])
    REFERENCES [dbo].[AuthTypeMasters]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserLogin_AuthTypeMaster'
CREATE INDEX [IX_FK_UserLogin_AuthTypeMaster]
ON [dbo].[UserLogins]
    ([AuthTypeId]);
GO

-- Creating foreign key on [CheckListId] in table 'ProjectCheckLists'
ALTER TABLE [dbo].[ProjectCheckLists]
ADD CONSTRAINT [FK_ProjectCheckList_CheckList]
    FOREIGN KEY ([CheckListId])
    REFERENCES [dbo].[CheckLists]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectCheckList_CheckList'
CREATE INDEX [IX_FK_ProjectCheckList_CheckList]
ON [dbo].[ProjectCheckLists]
    ([CheckListId]);
GO

-- Creating foreign key on [StatedId] in table 'Cities'
ALTER TABLE [dbo].[Cities]
ADD CONSTRAINT [FK_MasterCity_MasterState]
    FOREIGN KEY ([StatedId])
    REFERENCES [dbo].[States]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterCity_MasterState'
CREATE INDEX [IX_FK_MasterCity_MasterState]
ON [dbo].[Cities]
    ([StatedId]);
GO

-- Creating foreign key on [CountryId] in table 'States'
ALTER TABLE [dbo].[States]
ADD CONSTRAINT [FK_MasterState_MasterContry]
    FOREIGN KEY ([CountryId])
    REFERENCES [dbo].[Countries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterState_MasterContry'
CREATE INDEX [IX_FK_MasterState_MasterContry]
ON [dbo].[States]
    ([CountryId]);
GO

-- Creating foreign key on [SmtpServerId] in table 'Emails'
ALTER TABLE [dbo].[Emails]
ADD CONSTRAINT [FK_Email_SmtpServer]
    FOREIGN KEY ([SmtpServerId])
    REFERENCES [dbo].[SmtpServers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Email_SmtpServer'
CREATE INDEX [IX_FK_Email_SmtpServer]
ON [dbo].[Emails]
    ([SmtpServerId]);
GO

-- Creating foreign key on [PushEmailEventId] in table 'EmailTemplates'
ALTER TABLE [dbo].[EmailTemplates]
ADD CONSTRAINT [FK_EmailTemplate_PushEmailEvent]
    FOREIGN KEY ([PushEmailEventId])
    REFERENCES [dbo].[PushEmailEvents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EmailTemplate_PushEmailEvent'
CREATE INDEX [IX_FK_EmailTemplate_PushEmailEvent]
ON [dbo].[EmailTemplates]
    ([PushEmailEventId]);
GO

-- Creating foreign key on [TenantId] in table 'EmailTemplates'
ALTER TABLE [dbo].[EmailTemplates]
ADD CONSTRAINT [FK_EmailTemplate_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EmailTemplate_Tenant'
CREATE INDEX [IX_FK_EmailTemplate_Tenant]
ON [dbo].[EmailTemplates]
    ([TenantId]);
GO

-- Creating foreign key on [EntityTypeId] in table 'Entities'
ALTER TABLE [dbo].[Entities]
ADD CONSTRAINT [FK_Entity_Entity]
    FOREIGN KEY ([EntityTypeId])
    REFERENCES [dbo].[EntityTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Entity_Entity'
CREATE INDEX [IX_FK_Entity_Entity]
ON [dbo].[Entities]
    ([EntityTypeId]);
GO

-- Creating foreign key on [EntitySubTypeId] in table 'Entities'
ALTER TABLE [dbo].[Entities]
ADD CONSTRAINT [FK_Entity_EntityType]
    FOREIGN KEY ([EntitySubTypeId])
    REFERENCES [dbo].[EntitySubTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Entity_EntityType'
CREATE INDEX [IX_FK_Entity_EntityType]
ON [dbo].[Entities]
    ([EntitySubTypeId]);
GO

-- Creating foreign key on [TenantId] in table 'Entities'
ALTER TABLE [dbo].[Entities]
ADD CONSTRAINT [FK_Entity_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Entity_Tenant'
CREATE INDEX [IX_FK_Entity_Tenant]
ON [dbo].[Entities]
    ([TenantId]);
GO

-- Creating foreign key on [EntityId] in table 'EntityVariables'
ALTER TABLE [dbo].[EntityVariables]
ADD CONSTRAINT [FK_EntityVariable_Entity]
    FOREIGN KEY ([EntityId])
    REFERENCES [dbo].[Entities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntityVariable_Entity'
CREATE INDEX [IX_FK_EntityVariable_Entity]
ON [dbo].[EntityVariables]
    ([EntityId]);
GO

-- Creating foreign key on [VariableId] in table 'EntityFormDataVariables'
ALTER TABLE [dbo].[EntityFormDataVariables]
ADD CONSTRAINT [FK_EntityDataVariable_Variable]
    FOREIGN KEY ([VariableId])
    REFERENCES [dbo].[Variables]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntityDataVariable_Variable'
CREATE INDEX [IX_FK_EntityDataVariable_Variable]
ON [dbo].[EntityFormDataVariables]
    ([VariableId]);
GO

-- Creating foreign key on [EntityTypeId] in table 'EntitySubTypes'
ALTER TABLE [dbo].[EntitySubTypes]
ADD CONSTRAINT [FK_EntitySubType_EntityType]
    FOREIGN KEY ([EntityTypeId])
    REFERENCES [dbo].[EntityTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntitySubType_EntityType'
CREATE INDEX [IX_FK_EntitySubType_EntityType]
ON [dbo].[EntitySubTypes]
    ([EntityTypeId]);
GO

-- Creating foreign key on [TenantId] in table 'EntityTypes'
ALTER TABLE [dbo].[EntityTypes]
ADD CONSTRAINT [FK_EntityType_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntityType_Tenant'
CREATE INDEX [IX_FK_EntityType_Tenant]
ON [dbo].[EntityTypes]
    ([TenantId]);
GO

-- Creating foreign key on [EntityTypeId] in table 'FormEntityTypes'
ALTER TABLE [dbo].[FormEntityTypes]
ADD CONSTRAINT [FK_FormEntityType_EntityType]
    FOREIGN KEY ([EntityTypeId])
    REFERENCES [dbo].[EntityTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormEntityType_EntityType'
CREATE INDEX [IX_FK_FormEntityType_EntityType]
ON [dbo].[FormEntityTypes]
    ([EntityTypeId]);
GO

-- Creating foreign key on [EntityTypeId] in table 'VariableEntityTypes'
ALTER TABLE [dbo].[VariableEntityTypes]
ADD CONSTRAINT [FK_MasterVariableEntityType_EntityType]
    FOREIGN KEY ([EntityTypeId])
    REFERENCES [dbo].[EntityTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterVariableEntityType_EntityType'
CREATE INDEX [IX_FK_MasterVariableEntityType_EntityType]
ON [dbo].[VariableEntityTypes]
    ([EntityTypeId]);
GO

-- Creating foreign key on [EntityTypeId] in table 'ProjectEntityTypes'
ALTER TABLE [dbo].[ProjectEntityTypes]
ADD CONSTRAINT [FK_ProjectEntityType_EntityType]
    FOREIGN KEY ([EntityTypeId])
    REFERENCES [dbo].[EntityTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectEntityType_EntityType'
CREATE INDEX [IX_FK_ProjectEntityType_EntityType]
ON [dbo].[ProjectEntityTypes]
    ([EntityTypeId]);
GO

-- Creating foreign key on [VariableId] in table 'EntityVariables'
ALTER TABLE [dbo].[EntityVariables]
ADD CONSTRAINT [FK_EntityVariable_Variable]
    FOREIGN KEY ([VariableId])
    REFERENCES [dbo].[Variables]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EntityVariable_Variable'
CREATE INDEX [IX_FK_EntityVariable_Variable]
ON [dbo].[EntityVariables]
    ([VariableId]);
GO

-- Creating foreign key on [TenantId] in table 'Forms'
ALTER TABLE [dbo].[Forms]
ADD CONSTRAINT [FK_Form_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Form_Tenant'
CREATE INDEX [IX_FK_Form_Tenant]
ON [dbo].[Forms]
    ([TenantId]);
GO

-- Creating foreign key on [FormId] in table 'FormEntityTypes'
ALTER TABLE [dbo].[FormEntityTypes]
ADD CONSTRAINT [FK_FormEntityType_Form]
    FOREIGN KEY ([FormId])
    REFERENCES [dbo].[Forms]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormEntityType_Form'
CREATE INDEX [IX_FK_FormEntityType_Form]
ON [dbo].[FormEntityTypes]
    ([FormId]);
GO

-- Creating foreign key on [FormCategoryId] in table 'Forms'
ALTER TABLE [dbo].[Forms]
ADD CONSTRAINT [FK_Forms_FormCategory]
    FOREIGN KEY ([FormCategoryId])
    REFERENCES [dbo].[FormCategories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Forms_FormCategory'
CREATE INDEX [IX_FK_Forms_FormCategory]
ON [dbo].[Forms]
    ([FormCategoryId]);
GO

-- Creating foreign key on [FormStatusId] in table 'Forms'
ALTER TABLE [dbo].[Forms]
ADD CONSTRAINT [FK_Forms_FormStatus]
    FOREIGN KEY ([FormStatusId])
    REFERENCES [dbo].[FormStatus]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Forms_FormStatus'
CREATE INDEX [IX_FK_Forms_FormStatus]
ON [dbo].[Forms]
    ([FormStatusId]);
GO

-- Creating foreign key on [FormId] in table 'FormVariables'
ALTER TABLE [dbo].[FormVariables]
ADD CONSTRAINT [FK_FormVariable_Forms]
    FOREIGN KEY ([FormId])
    REFERENCES [dbo].[Forms]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormVariable_Forms'
CREATE INDEX [IX_FK_FormVariable_Forms]
ON [dbo].[FormVariables]
    ([FormId]);
GO

-- Creating foreign key on [FormId] in table 'ReviewerFeedbacks'
ALTER TABLE [dbo].[ReviewerFeedbacks]
ADD CONSTRAINT [FK_ReviewerFeedback_Form]
    FOREIGN KEY ([FormId])
    REFERENCES [dbo].[Forms]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ReviewerFeedback_Form'
CREATE INDEX [IX_FK_ReviewerFeedback_Form]
ON [dbo].[ReviewerFeedbacks]
    ([FormId]);
GO

-- Creating foreign key on [TenantId] in table 'FormCategories'
ALTER TABLE [dbo].[FormCategories]
ADD CONSTRAINT [FK_FormCategory_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormCategory_Tenant'
CREATE INDEX [IX_FK_FormCategory_Tenant]
ON [dbo].[FormCategories]
    ([TenantId]);
GO

-- Creating foreign key on [ProjectId] in table 'FormDataEntries'
ALTER TABLE [dbo].[FormDataEntries]
ADD CONSTRAINT [FK_FormDataEntry_Project]
    FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormDataEntry_Project'
CREATE INDEX [IX_FK_FormDataEntry_Project]
ON [dbo].[FormDataEntries]
    ([ProjectId]);
GO

-- Creating foreign key on [FormDataEntryId] in table 'FormDataEntryVariables'
ALTER TABLE [dbo].[FormDataEntryVariables]
ADD CONSTRAINT [FK_FormDataEntryVariable_FormDataEntry]
    FOREIGN KEY ([FormDataEntryId])
    REFERENCES [dbo].[FormDataEntries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormDataEntryVariable_FormDataEntry'
CREATE INDEX [IX_FK_FormDataEntryVariable_FormDataEntry]
ON [dbo].[FormDataEntryVariables]
    ([FormDataEntryId]);
GO

-- Creating foreign key on [VariableId] in table 'FormDataEntryVariables'
ALTER TABLE [dbo].[FormDataEntryVariables]
ADD CONSTRAINT [FK_FormDataEntryVariable_MasterVariable]
    FOREIGN KEY ([VariableId])
    REFERENCES [dbo].[Variables]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormDataEntryVariable_MasterVariable'
CREATE INDEX [IX_FK_FormDataEntryVariable_MasterVariable]
ON [dbo].[FormDataEntryVariables]
    ([VariableId]);
GO

-- Creating foreign key on [VariableId] in table 'FormVariables'
ALTER TABLE [dbo].[FormVariables]
ADD CONSTRAINT [FK_FormVariable_MasterVariable]
    FOREIGN KEY ([VariableId])
    REFERENCES [dbo].[Variables]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormVariable_MasterVariable'
CREATE INDEX [IX_FK_FormVariable_MasterVariable]
ON [dbo].[FormVariables]
    ([VariableId]);
GO

-- Creating foreign key on [FormVariableId] in table 'FormVariableRoles'
ALTER TABLE [dbo].[FormVariableRoles]
ADD CONSTRAINT [FK_FormVariableRole_FormVariable]
    FOREIGN KEY ([FormVariableId])
    REFERENCES [dbo].[FormVariables]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormVariableRole_FormVariable'
CREATE INDEX [IX_FK_FormVariableRole_FormVariable]
ON [dbo].[FormVariableRoles]
    ([FormVariableId]);
GO

-- Creating foreign key on [RoleId] in table 'FormVariableRoles'
ALTER TABLE [dbo].[FormVariableRoles]
ADD CONSTRAINT [FK_FormVariableRole_Role]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormVariableRole_Role'
CREATE INDEX [IX_FK_FormVariableRole_Role]
ON [dbo].[FormVariableRoles]
    ([RoleId]);
GO

-- Creating foreign key on [VariableTypeId] in table 'LookupDatas'
ALTER TABLE [dbo].[LookupDatas]
ADD CONSTRAINT [FK_LookupData_VariableType]
    FOREIGN KEY ([VariableTypeId])
    REFERENCES [dbo].[VariableTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LookupData_VariableType'
CREATE INDEX [IX_FK_LookupData_VariableType]
ON [dbo].[LookupDatas]
    ([VariableTypeId]);
GO

-- Creating foreign key on [LookupDataId] in table 'LookupDataOptionGroups'
ALTER TABLE [dbo].[LookupDataOptionGroups]
ADD CONSTRAINT [FK_LookupDataOptionGroup_LookupData]
    FOREIGN KEY ([LookupDataId])
    REFERENCES [dbo].[LookupDatas]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LookupDataOptionGroup_LookupData'
CREATE INDEX [IX_FK_LookupDataOptionGroup_LookupData]
ON [dbo].[LookupDataOptionGroups]
    ([LookupDataId]);
GO

-- Creating foreign key on [LookupDataOptionGroupId] in table 'LookupDataGroupOptions'
ALTER TABLE [dbo].[LookupDataGroupOptions]
ADD CONSTRAINT [FK_LookupDataGroupOption_LookupDataOptionGroup]
    FOREIGN KEY ([LookupDataOptionGroupId])
    REFERENCES [dbo].[LookupDataOptionGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LookupDataGroupOption_LookupDataOptionGroup'
CREATE INDEX [IX_FK_LookupDataGroupOption_LookupDataOptionGroup]
ON [dbo].[LookupDataGroupOptions]
    ([LookupDataOptionGroupId]);
GO

-- Creating foreign key on [PrivilegeId] in table 'RolePrivileges'
ALTER TABLE [dbo].[RolePrivileges]
ADD CONSTRAINT [FK_RolePrivilege_Privilege]
    FOREIGN KEY ([PrivilegeId])
    REFERENCES [dbo].[Privileges]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RolePrivilege_Privilege'
CREATE INDEX [IX_FK_RolePrivilege_Privilege]
ON [dbo].[RolePrivileges]
    ([PrivilegeId]);
GO

-- Creating foreign key on [PreviousProjectId] in table 'Projects'
ALTER TABLE [dbo].[Projects]
ADD CONSTRAINT [FK_Project_Project]
    FOREIGN KEY ([PreviousProjectId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Project_Project'
CREATE INDEX [IX_FK_Project_Project]
ON [dbo].[Projects]
    ([PreviousProjectId]);
GO

-- Creating foreign key on [ProjectStatusId] in table 'Projects'
ALTER TABLE [dbo].[Projects]
ADD CONSTRAINT [FK_Project_ProjectStatus]
    FOREIGN KEY ([ProjectStatusId])
    REFERENCES [dbo].[ProjectStatus]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Project_ProjectStatus'
CREATE INDEX [IX_FK_Project_ProjectStatus]
ON [dbo].[Projects]
    ([ProjectStatusId]);
GO

-- Creating foreign key on [TenantId] in table 'Projects'
ALTER TABLE [dbo].[Projects]
ADD CONSTRAINT [FK_Project_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Project_Tenant'
CREATE INDEX [IX_FK_Project_Tenant]
ON [dbo].[Projects]
    ([TenantId]);
GO

-- Creating foreign key on [ProjectId] in table 'ProjectCheckLists'
ALTER TABLE [dbo].[ProjectCheckLists]
ADD CONSTRAINT [FK_ProjectCheckList_Project]
    FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectCheckList_Project'
CREATE INDEX [IX_FK_ProjectCheckList_Project]
ON [dbo].[ProjectCheckLists]
    ([ProjectId]);
GO

-- Creating foreign key on [ProjectId] in table 'ProjectEntityTypes'
ALTER TABLE [dbo].[ProjectEntityTypes]
ADD CONSTRAINT [FK_ProjectEntityType_Project]
    FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectEntityType_Project'
CREATE INDEX [IX_FK_ProjectEntityType_Project]
ON [dbo].[ProjectEntityTypes]
    ([ProjectId]);
GO

-- Creating foreign key on [ProjectId] in table 'ProjectParticipants'
ALTER TABLE [dbo].[ProjectParticipants]
ADD CONSTRAINT [FK_ProjectParticipant_Project]
    FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectParticipant_Project'
CREATE INDEX [IX_FK_ProjectParticipant_Project]
ON [dbo].[ProjectParticipants]
    ([ProjectId]);
GO

-- Creating foreign key on [ProjectId] in table 'ProjectRoles'
ALTER TABLE [dbo].[ProjectRoles]
ADD CONSTRAINT [FK_ProjectRole_Project]
    FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectRole_Project'
CREATE INDEX [IX_FK_ProjectRole_Project]
ON [dbo].[ProjectRoles]
    ([ProjectId]);
GO

-- Creating foreign key on [ProjectId] in table 'ProjectSites'
ALTER TABLE [dbo].[ProjectSites]
ADD CONSTRAINT [FK_ProjectSite_Project]
    FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectSite_Project'
CREATE INDEX [IX_FK_ProjectSite_Project]
ON [dbo].[ProjectSites]
    ([ProjectId]);
GO

-- Creating foreign key on [ProjectId] in table 'ProjectStaffMembers'
ALTER TABLE [dbo].[ProjectStaffMembers]
ADD CONSTRAINT [FK_ProjectStaffMember_Project]
    FOREIGN KEY ([ProjectId])
    REFERENCES [dbo].[Projects]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectStaffMember_Project'
CREATE INDEX [IX_FK_ProjectStaffMember_Project]
ON [dbo].[ProjectStaffMembers]
    ([ProjectId]);
GO

-- Creating foreign key on [RoleId] in table 'ProjectRoles'
ALTER TABLE [dbo].[ProjectRoles]
ADD CONSTRAINT [FK_ProjectRole_Role]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectRole_Role'
CREATE INDEX [IX_FK_ProjectRole_Role]
ON [dbo].[ProjectRoles]
    ([RoleId]);
GO

-- Creating foreign key on [SiteId] in table 'ProjectSites'
ALTER TABLE [dbo].[ProjectSites]
ADD CONSTRAINT [FK_ProjectSite_Site]
    FOREIGN KEY ([SiteId])
    REFERENCES [dbo].[Sites]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectSite_Site'
CREATE INDEX [IX_FK_ProjectSite_Site]
ON [dbo].[ProjectSites]
    ([SiteId]);
GO

-- Creating foreign key on [UserId] in table 'ProjectStaffMembers'
ALTER TABLE [dbo].[ProjectStaffMembers]
ADD CONSTRAINT [FK_ProjectStaffMember_UserLogin]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[UserLogins]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProjectStaffMember_UserLogin'
CREATE INDEX [IX_FK_ProjectStaffMember_UserLogin]
ON [dbo].[ProjectStaffMembers]
    ([UserId]);
GO

-- Creating foreign key on [RegExId] in table 'ValidationRules'
ALTER TABLE [dbo].[ValidationRules]
ADD CONSTRAINT [FK_ValidationRule_RegExRule]
    FOREIGN KEY ([RegExId])
    REFERENCES [dbo].[RegExRules]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ValidationRule_RegExRule'
CREATE INDEX [IX_FK_ValidationRule_RegExRule]
ON [dbo].[ValidationRules]
    ([RegExId]);
GO

-- Creating foreign key on [RoleId] in table 'VariableRoles'
ALTER TABLE [dbo].[VariableRoles]
ADD CONSTRAINT [FK_MasterVariableRole_Role]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterVariableRole_Role'
CREATE INDEX [IX_FK_MasterVariableRole_Role]
ON [dbo].[VariableRoles]
    ([RoleId]);
GO

-- Creating foreign key on [TenantId] in table 'Roles'
ALTER TABLE [dbo].[Roles]
ADD CONSTRAINT [FK_Role_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Role_Tenant'
CREATE INDEX [IX_FK_Role_Tenant]
ON [dbo].[Roles]
    ([TenantId]);
GO

-- Creating foreign key on [RoleId] in table 'RolePrivileges'
ALTER TABLE [dbo].[RolePrivileges]
ADD CONSTRAINT [FK_RolePrivilege_Role]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RolePrivilege_Role'
CREATE INDEX [IX_FK_RolePrivilege_Role]
ON [dbo].[RolePrivileges]
    ([RoleId]);
GO

-- Creating foreign key on [RoleId] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [FK_UserRole_Role]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRole_Role'
CREATE INDEX [IX_FK_UserRole_Role]
ON [dbo].[UserRoles]
    ([RoleId]);
GO

-- Creating foreign key on [SecurityQuestionId] in table 'UserLogins'
ALTER TABLE [dbo].[UserLogins]
ADD CONSTRAINT [FK_UserLogin_SecurityQuestion]
    FOREIGN KEY ([SecurityQuestionId])
    REFERENCES [dbo].[SecurityQuestions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserLogin_SecurityQuestion'
CREATE INDEX [IX_FK_UserLogin_SecurityQuestion]
ON [dbo].[UserLogins]
    ([SecurityQuestionId]);
GO

-- Creating foreign key on [TenantId] in table 'Sites'
ALTER TABLE [dbo].[Sites]
ADD CONSTRAINT [FK_Site_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Site_Tenant'
CREATE INDEX [IX_FK_Site_Tenant]
ON [dbo].[Sites]
    ([TenantId]);
GO

-- Creating foreign key on [TenantId] in table 'UserLogins'
ALTER TABLE [dbo].[UserLogins]
ADD CONSTRAINT [FK_UserLogin_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserLogin_Tenant'
CREATE INDEX [IX_FK_UserLogin_Tenant]
ON [dbo].[UserLogins]
    ([TenantId]);
GO

-- Creating foreign key on [TenantId] in table 'Variables'
ALTER TABLE [dbo].[Variables]
ADD CONSTRAINT [FK_Variable_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Variable_Tenant'
CREATE INDEX [IX_FK_Variable_Tenant]
ON [dbo].[Variables]
    ([TenantId]);
GO

-- Creating foreign key on [TenantId] in table 'VariableCategories'
ALTER TABLE [dbo].[VariableCategories]
ADD CONSTRAINT [FK_VariableCategory_Tenant]
    FOREIGN KEY ([TenantId])
    REFERENCES [dbo].[Tenants]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_VariableCategory_Tenant'
CREATE INDEX [IX_FK_VariableCategory_Tenant]
ON [dbo].[VariableCategories]
    ([TenantId]);
GO

-- Creating foreign key on [UserId] in table 'UserRoles'
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [FK_UserRole_UserLogin]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[UserLogins]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserRole_UserLogin'
CREATE INDEX [IX_FK_UserRole_UserLogin]
ON [dbo].[UserRoles]
    ([UserId]);
GO

-- Creating foreign key on [VariableId] in table 'VariableEntityTypes'
ALTER TABLE [dbo].[VariableEntityTypes]
ADD CONSTRAINT [FK_MasterVariableEntityType_MasterVariable]
    FOREIGN KEY ([VariableId])
    REFERENCES [dbo].[Variables]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterVariableEntityType_MasterVariable'
CREATE INDEX [IX_FK_MasterVariableEntityType_MasterVariable]
ON [dbo].[VariableEntityTypes]
    ([VariableId]);
GO

-- Creating foreign key on [VariableId] in table 'VariableRoles'
ALTER TABLE [dbo].[VariableRoles]
ADD CONSTRAINT [FK_MasterVariableRole_MasterVariable]
    FOREIGN KEY ([VariableId])
    REFERENCES [dbo].[Variables]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterVariableRole_MasterVariable'
CREATE INDEX [IX_FK_MasterVariableRole_MasterVariable]
ON [dbo].[VariableRoles]
    ([VariableId]);
GO

-- Creating foreign key on [VariableId] in table 'VariableValues'
ALTER TABLE [dbo].[VariableValues]
ADD CONSTRAINT [FK_MasterVariableValue_MasterVariable]
    FOREIGN KEY ([VariableId])
    REFERENCES [dbo].[Variables]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MasterVariableValue_MasterVariable'
CREATE INDEX [IX_FK_MasterVariableValue_MasterVariable]
ON [dbo].[VariableValues]
    ([VariableId]);
GO

-- Creating foreign key on [VariableCategoryId] in table 'Variables'
ALTER TABLE [dbo].[Variables]
ADD CONSTRAINT [FK_Variable_VariableCategory]
    FOREIGN KEY ([VariableCategoryId])
    REFERENCES [dbo].[VariableCategories]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Variable_VariableCategory'
CREATE INDEX [IX_FK_Variable_VariableCategory]
ON [dbo].[Variables]
    ([VariableCategoryId]);
GO

-- Creating foreign key on [VariableTypeId] in table 'Variables'
ALTER TABLE [dbo].[Variables]
ADD CONSTRAINT [FK_Variable_VariableType]
    FOREIGN KEY ([VariableTypeId])
    REFERENCES [dbo].[VariableTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Variable_VariableType'
CREATE INDEX [IX_FK_Variable_VariableType]
ON [dbo].[Variables]
    ([VariableTypeId]);
GO

-- Creating foreign key on [ValidationId] in table 'VariableValidationRules'
ALTER TABLE [dbo].[VariableValidationRules]
ADD CONSTRAINT [FK_VariableValidationRule_ValidationRule]
    FOREIGN KEY ([ValidationId])
    REFERENCES [dbo].[ValidationRules]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_VariableValidationRule_ValidationRule'
CREATE INDEX [IX_FK_VariableValidationRule_ValidationRule]
ON [dbo].[VariableValidationRules]
    ([ValidationId]);
GO

-- Creating foreign key on [VariableId] in table 'VariableValidationRules'
ALTER TABLE [dbo].[VariableValidationRules]
ADD CONSTRAINT [FK_VariableValidationRule_Variable]
    FOREIGN KEY ([VariableId])
    REFERENCES [dbo].[Variables]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_VariableValidationRule_Variable'
CREATE INDEX [IX_FK_VariableValidationRule_Variable]
ON [dbo].[VariableValidationRules]
    ([VariableId]);
GO

-- Creating foreign key on [VariableId] in table 'VariableApprovalLogs'
ALTER TABLE [dbo].[VariableApprovalLogs]
ADD CONSTRAINT [FK_VariableApprovalLog_Variable]
    FOREIGN KEY ([VariableId])
    REFERENCES [dbo].[Variables]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_VariableApprovalLog_Variable'
CREATE INDEX [IX_FK_VariableApprovalLog_Variable]
ON [dbo].[VariableApprovalLogs]
    ([VariableId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------