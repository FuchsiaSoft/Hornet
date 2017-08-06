
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/06/2017 14:42:49
-- Generated from EDMX file: C:\Users\Chris\Documents\GitHub\Hornet\src\Hornet.Model\HornetModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [HornetConfigDatabase];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_HashGroupHashEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HashEntries] DROP CONSTRAINT [FK_HashGroupHashEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_RegexGroupRegexEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RegexEntries] DROP CONSTRAINT [FK_RegexGroupRegexEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_SHA1_inherits_HashEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HashEntries_SHA1] DROP CONSTRAINT [FK_SHA1_inherits_HashEntry];
GO
IF OBJECT_ID(N'[dbo].[FK_SHA256_inherits_HashEntry]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HashEntries_SHA256] DROP CONSTRAINT [FK_SHA256_inherits_HashEntry];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[HashGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HashGroups];
GO
IF OBJECT_ID(N'[dbo].[HashEntries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HashEntries];
GO
IF OBJECT_ID(N'[dbo].[RegexGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegexGroups];
GO
IF OBJECT_ID(N'[dbo].[RegexEntries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegexEntries];
GO
IF OBJECT_ID(N'[dbo].[HashEntries_SHA1]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HashEntries_SHA1];
GO
IF OBJECT_ID(N'[dbo].[HashEntries_SHA256]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HashEntries_SHA256];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'HashGroups'
CREATE TABLE [dbo].[HashGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'HashEntries'
CREATE TABLE [dbo].[HashEntries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HashValue] nvarchar(max)  NOT NULL,
    [Remarks] nvarchar(max)  NULL,
    [HashGroupId] int  NOT NULL
);
GO

-- Creating table 'RegexGroups'
CREATE TABLE [dbo].[RegexGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'RegexEntries'
CREATE TABLE [dbo].[RegexEntries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Pattern] nvarchar(max)  NOT NULL,
    [Remarks] nvarchar(max)  NOT NULL,
    [RegexGroupId] int  NOT NULL
);
GO

-- Creating table 'MD5'
CREATE TABLE [dbo].[MD5] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HashValue] nvarchar(max)  NULL,
    [Remarks] nvarchar(max)  NULL,
    [HashGroupId] int  NOT NULL
);
GO

-- Creating table 'HashEntries_SHA1'
CREATE TABLE [dbo].[HashEntries_SHA1] (
    [Id] int  NOT NULL
);
GO

-- Creating table 'HashEntries_SHA256'
CREATE TABLE [dbo].[HashEntries_SHA256] (
    [Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'HashGroups'
ALTER TABLE [dbo].[HashGroups]
ADD CONSTRAINT [PK_HashGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'HashEntries'
ALTER TABLE [dbo].[HashEntries]
ADD CONSTRAINT [PK_HashEntries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RegexGroups'
ALTER TABLE [dbo].[RegexGroups]
ADD CONSTRAINT [PK_RegexGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RegexEntries'
ALTER TABLE [dbo].[RegexEntries]
ADD CONSTRAINT [PK_RegexEntries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MD5'
ALTER TABLE [dbo].[MD5]
ADD CONSTRAINT [PK_MD5]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'HashEntries_SHA1'
ALTER TABLE [dbo].[HashEntries_SHA1]
ADD CONSTRAINT [PK_HashEntries_SHA1]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'HashEntries_SHA256'
ALTER TABLE [dbo].[HashEntries_SHA256]
ADD CONSTRAINT [PK_HashEntries_SHA256]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [HashGroupId] in table 'HashEntries'
ALTER TABLE [dbo].[HashEntries]
ADD CONSTRAINT [FK_HashGroupHashEntry]
    FOREIGN KEY ([HashGroupId])
    REFERENCES [dbo].[HashGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_HashGroupHashEntry'
CREATE INDEX [IX_FK_HashGroupHashEntry]
ON [dbo].[HashEntries]
    ([HashGroupId]);
GO

-- Creating foreign key on [RegexGroupId] in table 'RegexEntries'
ALTER TABLE [dbo].[RegexEntries]
ADD CONSTRAINT [FK_RegexGroupRegexEntry]
    FOREIGN KEY ([RegexGroupId])
    REFERENCES [dbo].[RegexGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RegexGroupRegexEntry'
CREATE INDEX [IX_FK_RegexGroupRegexEntry]
ON [dbo].[RegexEntries]
    ([RegexGroupId]);
GO

-- Creating foreign key on [HashGroupId] in table 'MD5'
ALTER TABLE [dbo].[MD5]
ADD CONSTRAINT [FK_HashGroupMD5]
    FOREIGN KEY ([HashGroupId])
    REFERENCES [dbo].[HashGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_HashGroupMD5'
CREATE INDEX [IX_FK_HashGroupMD5]
ON [dbo].[MD5]
    ([HashGroupId]);
GO

-- Creating foreign key on [Id] in table 'HashEntries_SHA1'
ALTER TABLE [dbo].[HashEntries_SHA1]
ADD CONSTRAINT [FK_SHA1_inherits_HashEntry]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[HashEntries]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'HashEntries_SHA256'
ALTER TABLE [dbo].[HashEntries_SHA256]
ADD CONSTRAINT [FK_SHA256_inherits_HashEntry]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[HashEntries]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------