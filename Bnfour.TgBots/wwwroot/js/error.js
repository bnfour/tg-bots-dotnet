// imagine using raw js in this day and age

window.addEventListener("load", () => {
    // wait a bit before changing
    setTimeout(() => do_stuff(), 1000);
});

function do_stuff() {
    const arr = shuffle([...Array(code.length).keys()]);
    const span = document.getElementById("tspan7");

    for (var i = 0; i < arr.length; i++)
    {
        // smh my head
        (ind => setTimeout(() => replace(span, arr[ind], code), ind * 1200))(i);
    }
}

function replace(element, index, source) {
    element.innerHTML = setCharAt(element.innerHTML, index, source[index]);
}

function shuffle(array) {
    let currentIndex = array.length, randomIndex;
    while (currentIndex > 0) {
        randomIndex = Math.floor(Math.random() * currentIndex);
        currentIndex--;
        [array[currentIndex], array[randomIndex]] = [array[randomIndex], array[currentIndex]];
    }
    return array;
}

function setCharAt(str, idx, newChr) {
    return str.substring(0, idx) + newChr + str.substring(idx + 1);
}
