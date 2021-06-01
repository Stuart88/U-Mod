

window.dodgerInitGame = (instance) => {
    var canvasContainer = document.getElementById('canvasContainer'),
        canvases = canvasContainer.getElementsByTagName('canvas') || [];
    window.dodgerGame = {
        instance: instance,
        canvas: canvases.length ? canvases[0] : null
    };

    window.addEventListener("resize", dodgerOnResize);
    dodgerOnResize();

    window.requestAnimationFrame(dodgerGameLoop);
};

function dodgerGameLoop(timeStamp) {
    window.requestAnimationFrame(dodgerGameLoop);
    dodgerGame.instance.invokeMethodAsync('DodgerGameLoop', timeStamp, dodgerGame.canvas.width, dodgerGame.canvas.height);
}

function dodgerOnResize() {
    if (!window.dodgerGame.canvas)
        return;

    var container = document.getElementById('canvasContainer');

    dodgerGame.canvas.width = container.offsetWidth;
    dodgerGame.canvas.height = container.offsetHeight;
}