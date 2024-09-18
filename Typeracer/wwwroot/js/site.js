function startGame(){
    var audio = new Audio('/sounds/click.mp3');
    audio.play();

    audio.onended = function() {
        location.href = '/Home/Type';
    };
    
    console.log("Žaidimas pradėtas");
}

document.addEventListener('DOMContentLoaded', async (event) => {
    // http GET request from "/Home/GetParagraphText/"
    let response = await fetch('/Home/GetParagraphText/');
    let jsonResponse = await response.json();
    
    let typingText = jsonResponse.text;
    let typingTextElement = document.getElementById('typing-text');

    // sets the text of the paragraph to be the randomised paragraph
    typingTextElement.innerText = typingText;


    // now we need to listen for any typing in the textarea
    
    let currentIndex = 0;
    
    document.addEventListener('keydown', (event) => {
        let inputCharacter = event.key;
        
        // comparing the character to the text
        if (currentIndex < typingText.length && inputCharacter === typingText[currentIndex]) {
            let typedText = typingText.substring(0, currentIndex + 1); // correctly typed portion
            let remainingText = typingText.substring(currentIndex + 1); // untyped portion
            typingTextElement.innerHTML = `<span style="color: green;">${typedText}</span>${remainingText}`;
            currentIndex++;
        }
        
    });

});
    
    
    