ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [metabase_log], FILENAME = '$(Path1)MIGRATION_TEST_metabase_1.ldf', MAXSIZE = 2097152 MB, FILEGROWTH = 10 %);

