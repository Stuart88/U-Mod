

window.getFile = function (filename) {
    var link = document.createElement('a');
    link.href = `/Download/GetFile/${this.encodeURIComponent(filename)}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}