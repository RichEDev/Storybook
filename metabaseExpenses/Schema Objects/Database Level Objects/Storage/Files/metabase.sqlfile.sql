ALTER DATABASE [$(DatabaseName)]
    ADD FILE (NAME = [metabase], FILENAME = '$(Path2)MIGRATION_TEST_metabase.mdf', FILEGROWTH = 1024 KB) TO FILEGROUP [PRIMARY];

