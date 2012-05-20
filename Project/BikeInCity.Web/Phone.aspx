<%@ Page Title="" Language="C#" MasterPageFile="~/BikeMaster.Master" AutoEventWireup="true"
    Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<asp:Content ID="headerContent" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link href="css/mobile.css" rel="stylesheet" type="text/css" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="content_text">
    <p>It would be cool to have one application for each platform. If you are missing one for your platform, than just developt it! Check the project on GitHub.</p>
        <table id="mobile">
            <tbody>
                <!-- Results table headers -->
                <tr>
                    <th>
                    </th>
                    <th>
                        <img src="img/wp7.png" />
                    </th>
                    <th>
                        <img src="img/iphone.png" />
                    </th>
                    <th>
                        <img src="img/android.png" />
                    </th>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <a href="#api" class="not_active">Develop it</a>
                        <!--<a href="http://social.zune.net/redirect?type=phoneApp&id=920707d6-5505-e011-9264-00237de2db9e"
                            class="active">Get from Market</a>-->
                    </td>
                    <td>
                        <a href="#api" class="not_active">Develop it</a>
                    </td>
                    <td>
                        <a href="#api" class="not_active">Develop it</a>
                    </td>
                </tr>
            </tbody>
        </table>
        <div class="line">
        </div>
        <div id="showcase">
            <div class="logo">
                <img src="img/wp7.png" /></div>
            <div id="pic">
            </div>
            <div class="functions">
                <div>
                    <a href="#" onclick="desc(1)">>> select the city</a></div>
                <div>
                    <a href="#" onclick="desc(2)">>> show 5 nearest stations</a></div>
                <div>
                    <a href="#" onclick="desc(3)">>> location search function</a></div>
                <div>
                    <a href="#" onclick="desc(4)">>> get biking direction</a></div>
                <div>
                    <a href="#" onclick="desc(5)">>> all stations in city</a></div>
                <div>
                    <a href="#" onclick="desc(6)">>> change of settings</a></div>
                <div id="description"></div>
            </div>
        </div>
        <p>The applciation for WP7 is almost ready. Just needs someone with Windows Phone to finish the deployment of the application! Let me know if you are interested!</p>

         <div class="line">
         </div>

         <p></p>
        <script type="text/javascript">
            function desc(id) {
                var auxImageName;
                var auxText;

                if (id == 1) {
                    auxImageName = '<img src="img/wp7/city.png" />';
                    auxText = 'The first time you start the application you should be prompted to selected your city. After selecting your city, the list of all bike stations of the city will be loaded.';
                }

                if (id == 2) {
                    auxImageName = '<img src="img/wp7/5nearest.png" />';
                    auxText = 'After the stations are loaded you can press the middle button of the Application Bar which will show 5 nearest stations to your actuall position. Note that this function uses the GPS of your telefon, so it should not be disabled.';
                }

                if (id == 3) {
                    auxImageName = '<img src="img/wp7/find.png">';
                    auxText = 'If you want to see stations near to some other place than is your actual location, press the Search button (first button from right in the application bar).';
                }

                if (id == 4) {
                    auxImageName = '<img src="img/wp7/directions.png">';
                    auxText = 'The last functionality lets you to get biking directions. Select one of the stations on your map (simply by clicking) and than press the "directions" button (first from the left in the application bar). You will be asked to enter address of your destination.';
                }

                if (id == 5) {
                    auxImageName = '<img src="img/wp7/allstations.png">';
                    auxText = 'Alternatively you can also visualize on the map all the stations in the city.';
                }

                if (id == 6) {
                    auxImageName = '<img src="img/wp7/settings.png">';
                    auxText = 'To show the menu click on the three dots on the application bar. By clicking on the "change city" menu a list of cities will be showed which will let you select another city. "turn off gps/turn on gps" button will just turn off or on the receiving of the GPS signal.';
                }

                var auxImageDiv = document.getElementById("pic");
                auxImageDiv.innerHTML = auxImageName;

                var auxTextDiv = document.getElementById("description");
                auxTextDiv.innerHTML = auxText;
            }

            desc(1);
        </script>
    </div>
</asp:Content>
