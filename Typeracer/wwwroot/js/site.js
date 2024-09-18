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
    let errorIndex = -1;
    
    document.addEventListener('keydown', (event) => {
        let inputCharacter = event.key;

        // handling delete key
        if (inputCharacter === "Backspace") {
            if (currentIndex > 0) {
                --currentIndex;
            }

            // if the user has come back to the error index
            if (currentIndex <= errorIndex) {
                errorIndex = -1;
            }

            displayColorOverlay();
            return;

        }

        // we must ignore shift and other keys non-character keys, since they should not count
        if (inputCharacter.length > 1) {
            return;
        }
        
        if (currentIndex < typingText.length) {
            
            if (inputCharacter !== typingText[currentIndex] && errorIndex === -1) {
                errorIndex = currentIndex;
            }
            
            ++currentIndex;
            
            displayColorOverlay();

        }


        function displayColorOverlay() {
            let displayedText = "";

            if (errorIndex === -1) {
                let typedText = typingText.substring(0, currentIndex);
                let remainingText = typingText.substring(currentIndex);
                displayedText = `<span style="color: green;">${typedText}</span>${remainingText}`;
            } else {
                let correctlyTypedText = typingText.substring(0, errorIndex);
                let incorrectlyTypedText = typingText.substring(errorIndex, currentIndex);
                let remainingUntypedText = typingText.substring(currentIndex);

                displayedText = `
                    <span style="color: green;">${correctlyTypedText}</span>
                    <span style="color: red;">${incorrectlyTypedText}</span>${remainingUntypedText}`;
                
            }
            typingTextElement.innerHTML = displayedText;

        }

    });
});
    
    
    