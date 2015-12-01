function ajaxpost() {
    alert('asdf');
    var mypostrequest = new ajaxRequest();
    //mypostrequest.onreadystatechange = function () {
    //    if (mypostrequest.readyState == 4) {
    //        if (mypostrequest.status == 200 || window.location.href.indexOf("http") == -1) {
    //            document.getElementById("result").innerHTML = mypostrequest.responseText;
    //        }
    //        else {
    //            alert("An error has occured making the request");
    //        }
    //    }
    //}

    var parameters = "email=soccercjs2%40gmail.com&password=united2";
    mypostrequest.open("POST", "http://www.fleaflicker.com/nfl/login", true);
    mypostrequest.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    mypostrequest.send(parameters);
}

function ajaxRequest() {
    var activexmodes = ["Msxml2.XMLHTTP", "Microsoft.XMLHTTP"] //activeX versions to check for in IE
    if (window.ActiveXObject) { //Test for support for ActiveXObject in IE first (as XMLHttpRequest in IE7 is broken)
        for (var i = 0; i < activexmodes.length; i++) {
            try {
                return new ActiveXObject(activexmodes[i])
            }
            catch (e) {
                //suppress error
            }
        }
    }
    else if (window.XMLHttpRequest) // if Mozilla, Safari etc
        return new XMLHttpRequest()
    else
        return false
}

function postLogin()
{
    var loginRequest = new XMLHttpRequest();
    var url = "http://www.fleaflicker.com/nfl/login";
    var params = "email=soccercjs2@gmail.com&password=united2";
    loginRequest.open("POST", url, true);

    //Send the proper header information along with the request
    loginRequest.setRequestHeader("Host", "www.fleaflicker.com");
    loginRequest.setRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:42.0) Gecko/20100101 Firefox/42.0");
    loginRequest.setRequestHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
    loginRequest.setRequestHeader("Accept-Language", "en-US,en;q=0.5");
    loginRequest.setRequestHeader("Accept-Encoding", "gzip, deflate");
    loginRequest.setRequestHeader("Referer", "http://www.fleaflicker.com/nfl/login");
    loginRequest.setRequestHeader("Connection", "keep-alive");
    loginRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    loginRequest.setRequestHeader("Content-length", params.length);
    //loginRequest.setRequestHeader("Access-Control-Expose-Headers", "Set-Cookie");
    loginRequest.withCredentials = true;
    loginRequest.send(params);

    var strCookie = loginRequest.getResponseHeader("Set-Cookie");

    alert(document.cookie);
    alert("<pre>Cookie from " + url + ": " + strCookie + "</pre>");

    var pageRequest = new XMLHttpRequest();
    var url = "http://www.fleaflicker.com/settings/admin";
    pageRequest.open("GET", url, true);
    pageRequest.setRequestHeader("Cookie", strCookie);
    pageRequest.withCredentials = true;
    pageRequest.send();

    alert("<pre>Cookie from " + url + ": " + pageRequest.responseText + "</pre>");

    //http.onreadystatechange = function () {//Call a function when the state changes.
    //    if (http.readyState == 4 && http.status == 303) {
    //        alert(http.responseText);
    //    }
    //}
}

function loginRequest()
{
    alert(1);
    var strPath = "http://www.fleaflicker.com/nfl/login";
    var strRequest = "email=soccercjs2%40gmail.com&password=united2";
    alert(2);
    var objHTTP = new XMLHttpRequest();
    alert(3);
    objHTTP.open = ("POST", strPath, false);
    alert(4);
    objHTTP.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    alert(5);
    objHTTP.send(strRequest);
    alert(6);

    var strCookie = objHTTP.getResponseHeader("Set-Cookie");

    alert("<pre>Cookie from " + strPath + ": " + strCookie + "</pre>");

    objHTTP = null;
    strPath = "http://www.fleaflicker.com/settings/admin";
    objHTTP = new XMLHttpRequest();
    objHTTP.open("GET", strPath, false);
    objHTTP.setRequestHeader("Cookie", strCookie);
    objHTTP.send();

    alert("<pre>Response from " + strPath + ": " + objHTTP.responseText + "</pre>");
    objHTTP = null;
}

function pageControl()
{

}