function startGame(){
    var audio = new Audio('/sounds/click.mp3');
    audio.play();

    audio.onended = function() {
        location.href = '/Home/Type';
    };
    
    console.log("Žaidimas pradėtas");
}