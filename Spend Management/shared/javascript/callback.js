		function getIndex(tblid, id)
		{
			var i;
			var tbl = document.getElementById(tblid);
			
			for (i = 0; i < tbl.rows.length; i++)
			{
				if (tbl.rows[i].id == id)
				{
					
					return i;
				}
			}
		}
				
		function doCallBack(url, dataToSend)
		{
			var xmlRequest;
			try
			{
				xmlRequest = new XMLHttpRequest();
			}
			catch (e)
			{	
				try
				{
					xmlRequest = new ActiveXObject("Microsoft.XMLHTTP");
				}
				catch (f)
				{
					xmlRequest = null;
				}
			}
			
			xmlRequest.open("POST",url,false);
			
			//xmlRequest.setRequestHeader("Content-Type","application/x-wais-source");
			xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
			xmlRequest.send(dataToSend);
			return xmlRequest.responseText;
		}
