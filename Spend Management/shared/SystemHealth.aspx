<%@ Page language="C#" Inherits="Spend_Management.shared.SystemHealth" MasterPageFile="~/masters/AuthenticatedUser.master" Codebehind="SystemHealth.aspx.cs" %>
<%@ MasterType VirtualPath="~/masters/AuthenticatedUser.master" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="contentmain">
        
    <script type="text/javascript" src="<%= Page.ResolveClientUrl("~/shared/javaScript/sel.systemHealth.js") %>"></script>

    <div id="iconPath" runat="server" ClientIDMode="Static" style="display: none"></div>

    <div id="healthPage">
        
        <div id="healthDialog">
            <div id="dialogInfo" style="max-height: 680px; max-width: 680px"></div>
        </div>
            
        <div id="healthTitle">System Health</div>

        <div id="generalInformation" class="informationBlock">
            <span class="title">General Information</span><img class="showHideButton" alt="Show or hide information" />
        </div>
        
        <div id="accountCacheInformation" class="informationBlock">
            <span class="title">Account Cache Keys</span><img id="imgShowAccountCacheKeys" src="<%= StaticPath %>/icons/16/plain/refresh.png" onclick="SEL.SystemHealth.ShowAccountCacheInfo()" class="btn" alt="Load accounts cache" /><img class="showHideButton" alt="Show or hide information" /><img id="resetAccountCache" alt="Update the cache from the database" src="<%= StaticPath %>/icons/16/plain/replace2.png" class="btn" onclick="SEL.SystemHealth.UpdateAccountCacheInfo()" />
            
            <div id="accountCacheKeys"></div>

        </div>
        
        <div id="cacheKeyInformation" class="informationBlock">
            <span class="title">Cache Keys</span><img id="imgShowCacheKeys" src="<%= StaticPath %>/icons/16/plain/refresh.png" onclick="SEL.SystemHealth.ShowExtendedHealthInfo()" class="btn" alt="Load running reports" /><img class="showHideButton" alt="Show or hide information" />
            
            <div id="cacheKeys"></div>
        </div>
        
        <div id="distributedCacheInformation" class="informationBlock">
            <span class="title">Distributed Cache Keys</span><img id="imgDistributedCacheKeys" src="<%= StaticPath %>/icons/16/plain/refresh.png" onclick="SEL.SystemHealth.ShowDistributedCacheInfo()" class="btn" alt="Distributed Cache Information" /><img class="showHideButton" alt="Show or hide information" /><img id="resetDistributedCache" alt="Clear the distributed cache" src="<%= StaticPath %>/icons/16/plain/replace2.png" class="btn" onclick="SEL.SystemHealth.ClearDistributedCache()" />
            
            <div id="distributedCache"></div>
        </div>
        <div id="distributedCacheStatisticsInformation" class="informationBlock">
            <span class="title">Distributed Cache Statistics</span><img id="img1" src="<%= StaticPath %>/icons/16/plain/refresh.png" onclick="SEL.SystemHealth.ShowDistributedCacheStatisticsInfo()" class="btn" alt="Distributed Cache Information" />
            
            <div id="distributedCacheStatistics"></div>
        </div>
              
        <div id="reportRequestInformation" class="informationBlock">
            <span class="title">Report Request Information</span><img id="imgShowReports" src="<%= StaticPath %>/icons/16/plain/refresh.png" onclick="SEL.SystemHealth.ShowRunningReportsInfo()" class="btn" alt="Load running reports" /><img class="showHideButton" alt="Show or hide information" />
            
            <div id="runningReportRequests"></div>
        </div>  
                       
        <div id="reportThreadInformation" class="informationBlock">   
            <span class="title">Report Thread Information</span><img id="imgShowThreads" src="<%= StaticPath %>/icons/16/plain/refresh.png" onclick="SEL.SystemHealth.ShowRunningReportsInfo()" class="btn" alt="Load running report threads" /><img class="showHideButton" alt="Show or hide information" />
            
            <div id="runningReportThreads"></div>
        </div>           
        <div id="financialReportInformation" class="informationBlock">   
            <span class="title">Financial Reports Information</span><img id="imgShowThreads" src="<%= StaticPath %>/icons/16/plain/refresh.png" onclick="SEL.SystemHealth.TestFinancialReports()" class="btn" alt="Load running report threads" /><img class="showHideButton" alt="Show or hide information" />
            
            <div id="financialReportsResults"></div>
        </div>           
               <div id="reportInformation" class="informationBlock">   
            <span class="title">Current Account Reports Information</span><img id="imgShowThreads" src="<%= StaticPath %>/icons/16/plain/refresh.png" onclick="SEL.SystemHealth.TestReports()" class="btn" alt="Load running report threads" /><img class="showHideButton" alt="Show or hide information" />
            
            <div id="reportsResults"></div>
        </div>         
        <div id="reportEventInformation" class="informationBlock">
            <span class="title">Expenses Reports Event Logs</span><img id="imgShowReportsEvents" src="<%= StaticPath %>/icons/16/plain/refresh.png" data-eventlog="2" class="btn healthpage-eventloader" alt="Load expenses reports event logs" /><img class="showHideButton" alt="Show or hide information" />
            <br/><input id="reportsMachine" type="text" maxlength="500" class="healthpage-input" placeholder="Machine Name"/><input id="reportsNumberOfEvents" type="text" maxlength="4" class="healthpage-input" placeholder="# of Events to show"/>
            
            <div id="reportsEventLogs" class="healthpage-events"></div>
        </div>
              
        <div id="fwReportEventInformation" class="informationBlock">
            <span class="title">Framework Reports Event Logs</span><img id="imgShowFwReportsEvents" src="<%= StaticPath %>/icons/16/plain/refresh.png" data-eventlog="3" class="btn healthpage-eventloader" alt="Load framework event logs" /><img class="showHideButton" alt="Show or hide information" />
            <br/><input id="fwReportsMachine" type="text" maxlength="500" class="healthpage-input" placeholder="Machine Name"/><input id="fwReportsNumberOfEvents" type="text" maxlength="4" class="healthpage-input" placeholder="# of Events to show"/>
            
            <div id="fwReportsEventLogs" class="healthpage-events"></div>
        </div>
              
        <div id="schedulerInformation" class="informationBlock">
            <span class="title">Expenses Scheduler Event Logs</span><img id="imgShowSchedulerEvents" src="<%= StaticPath %>/icons/16/plain/refresh.png" data-eventlog="4" class="btn healthpage-eventloader" alt="Load expenses scheduler event logs" /><img class="showHideButton" alt="Show or hide information" />
            <br/><input id="schedulerMachine" type="text" maxlength="500" class="healthpage-input" placeholder="Machine Name"/><input id="schedulerNumberOfEvents" type="text" maxlength="4" class="healthpage-input" placeholder="# of Events to show"/>
            
            <div id="schedulerEventLogs" class="healthpage-events"></div>
        </div>
              
        <div id="fwSchedulerInformation" class="informationBlock">
            <span class="title">Framework Scheduler Event Logs</span><img id="imgShowFwSchedulerEvents" src="<%= StaticPath %>/icons/16/plain/refresh.png" data-eventlog="5" class="btn healthpage-eventloader" alt="Load framework scheduler event logs" /><img class="showHideButton" alt="Show or hide information" />
                <br/><input id="fwSchedulerMachine" type="text" maxlength="500" class="healthpage-input" placeholder="Machine Name"/><input id="fwSchedulerNumberOfEvents" type="text" maxlength="4" class="healthpage-input" placeholder="# of Events to show"/>
            
            <div id="fwSchedulerEventLogs" class="healthpage-events"></div>
        </div>

        <div id="selApplicationInformation" class="informationBlock">
            <span class="title">SEL Application Event Logs</span><img id="imgShowSelApplicationEvents" src="<%= StaticPath %>/icons/16/plain/refresh.png" data-eventlog="1" class="btn healthpage-eventloader" alt="Load application event logs" /><img class="showHideButton" alt="Show or hide information" />
            <br/><input id="selApplicationMachine" type="text" maxlength="500" class="healthpage-input" placeholder="Machine Name"/><input id="selApplicationNumberOfEvents" type="text" maxlength="4" class="healthpage-input" placeholder="# of Events to show"/>
            
            <div id="selApplicationEventLogs" class="healthpage-events"></div>
        </div>

        <div id="esrNhsHubInformation" class="informationBlock">
            <span class="title">ESR NHS Hub Event Logs</span><img id="imgShowEsrNhsHubEvents" src="<%= StaticPath %>/icons/16/plain/refresh.png" data-eventlog="9" class="btn healthpage-eventloader" alt="Load application event logs" /><img class="showHideButton" alt="Show or hide information" />
            <br/><input id="esrNhsHubMachine" type="text" maxlength="500" class="healthpage-input" placeholder="Machine Name"/><input id="esrNhsHubNumberOfEvents" type="text" maxlength="4" class="healthpage-input" placeholder="# of Events to show"/>
            
            <div id="esrNhsHubEventLogs" class="healthpage-events"></div>
        </div>

        <div id="esrRouterInformation" class="informationBlock">
            <span class="title">ESR Router Event Logs</span><img id="imgShowEsrRouterEvents" src="<%= StaticPath %>/icons/16/plain/refresh.png" data-eventlog="10" class="btn healthpage-eventloader" alt="Load application event logs" /><img class="showHideButton" alt="Show or hide information" />
            <br/><input id="esrRouterMachine" type="text" maxlength="500" class="healthpage-input" placeholder="Machine Name"/><input id="esrRouterNumberOfEvents" type="text" maxlength="4" class="healthpage-input" placeholder="# of Events to show"/>
            
            <div id="esrRouterEventLogs" class="healthpage-events"></div>
        </div>
              
        <div id="applicationInformation" class="informationBlock">
            <span class="title">Application Event Logs</span><img id="imgShowApplicationEvents" src="<%= StaticPath %>/icons/16/plain/refresh.png" data-eventlog="7" class="btn healthpage-eventloader" alt="Load application event logs" /><img class="showHideButton" alt="Show or hide information" />
            <br/><input id="applicationMachine" type="text" maxlength="500" class="healthpage-input" placeholder="Machine Name"/><input id="applicationNumberOfEvents" type="text" maxlength="4" class="healthpage-input" placeholder="# of Events to show"/>
            
            <div id="applicationEventLogs" class="healthpage-events"></div>
        </div>
              
        <div id="systemInformation" class="informationBlock">
            <span class="title">System Event Logs</span><img id="imgShowSystemEvents" src="<%= StaticPath %>/icons/16/plain/refresh.png" data-eventlog="8" class="btn healthpage-eventloader" alt="Load application event logs" /><img class="showHideButton" alt="Show or hide information" />
            <br/><input id="systemMachine" type="text" maxlength="500" class="healthpage-input" placeholder="Machine Name"/><input id="systemNumberOfEvents" type="text" maxlength="4" class="healthpage-input" placeholder="# of Events to show"/>
            
            <div id="systemEventLogs" class="healthpage-events"></div>
        </div>


    </div>
    <style>
        #bowlG
        {
            position: relative;
            width: 20px;
            height: 20px;
        }

        #bowl_ringG
        {
            position: absolute;
            width: 20px;
            height: 20px;
            border: 4px solid <% = Colours.headerBGColour %>;
            -moz-border-radius: 20px;
            -webkit-border-radius: 20px;
            -o-border-radius: 20px;
            -ms-border-radius: 20px;
            border-radius: 20px;
        }

        .ball_holderG
        {
            position: absolute;
            width: 5px;
            height: 20px;
            left: 7px;
            top: 0px;
            -moz-animation-name: ball_moveG;
            -moz-animation-duration: 2s;
            -moz-animation-iteration-count: infinite;
            -moz-animation-timing-function: cubic-bezier(0.3, 0, 0.7, 1);
            -webkit-animation-name: ball_moveG;
            -webkit-animation-duration: 2s;
            -webkit-animation-iteration-count: infinite;
            -webkit-animation-timing-function: cubic-bezier(0.3, 0, 0.7, 1);
            -o-animation-name: ball_moveG;
            -o-animation-duration: 2s;
            -o-animation-iteration-count: infinite;
            -o-animation-timing-function: cubic-bezier(0.3, 0, 0.7, 1);
            -ms-animation-name: ball_moveG;
            -ms-animation-duration: 2s;
            -ms-animation-iteration-count: infinite;
            -ms-animation-timing-function: cubic-bezier(0.3, 0, 0.7, 1);
            animation-name: ball_moveG;
            animation-duration: 2s;
            animation-iteration-count: infinite;
            animation-timing-function: cubic-bezier(0.3, 0, 0.7, 1);/*cubic-bezier(0.8, 0.5, 0.7, 0.8);*/
        }

        .ballG
        {
            position: absolute;
            left: 0px;
            top: -13px;
            width: 6px;
            height: 6px;
            background: <% = Colours.headerBGColour %>;   
            -moz-border-radius: 6px;
            -webkit-border-radius: 6px;
            -o-border-radius: 6px;
            -ms-border-radius: 6px;
            border-radius: 6px;
        }

        @-moz-keyframes ball_moveG
        {
            0%
            {
                -moz-transform: rotate(330deg);
            }

            100%
            {
                -moz-transform: rotate(-30deg);
            }
        }

        @-webkit-keyframes ball_moveG
        {
            0%
            {
                -webkit-transform: rotate(330deg);
            }

            100%
            {
                -webkit-transform: rotate(-30deg);
            }
        }

        @-o-keyframes ball_moveG
        {
            0%
            {
                -o-transform: rotate(330deg);
            }

            100%
            {
                -o-transform: rotate(-30deg);
            }
        }

        @-ms-keyframes ball_moveG
        {
            0%
            {
                -ms-transform: rotate(330deg);
            }

            100%
            {
                -ms-transform: rotate(-30deg);
            }
        }

        @keyframes ball_moveG
        {
            0%
            {
                transform: rotate(330deg);
            }

            100%
            {
                transform: rotate(-30deg);
            }
        }

    </style>
    <div id="loaderHolder" style="display: none;">
        <div id="bowlG">
            <div id="bowl_ringG">
                <div class="ball_holderG">
                    <div class="ballG">
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>