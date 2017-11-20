ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [expenses_Data], FILENAME = '$(DefaultDataPath)$(DatabaseName).mdf', FILEGROWTH = 10 %) TO FILEGROUP [PRIMARY];

