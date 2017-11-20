EXECUTE sp_addmessage @msgnum = 777980450, @severity = 16, @msgtext = N'There is an existing pending action for agent %s. You must process this pending action before a new pending action can be inserted.', @lang = N'English';

