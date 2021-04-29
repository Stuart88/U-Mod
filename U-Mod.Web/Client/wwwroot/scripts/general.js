

window.getFile = function (filename) {
    var link = document.createElement('a');
    link.target = "_blank";
    link.href = `/Download/GetFile/${this.encodeURIComponent(filename)}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.openExternalUrlTab = function (url) {
    var link = document.createElement('a');
    link.target = "_blank";
    link.href = url;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function SetAddressBar(title, url) {
    if (typeof (history.pushState) != "undefined") {
        var obj = { Title: title, Url: url };
        history.pushState(obj, obj.Title, obj.Url);
    } else {
        console.log("SetAddressBar not supported: Browser does not support HTML5.");
    }
}