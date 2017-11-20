ALTER DATABASE [$(DatabaseName)]
    ADD LOG FILE (NAME = [expenses_Log], FILENAME = '$(DefaultLogPath)$(DatabaseName)_1.ldf', FILEGROWTH = 10 %);

