async function main() {
    const ROOM_URL = "https://signalrapp.daily.co/3zifqjBKtVJHHIZXoRsO";

    window.call = DailyIframe.createCallObject({
        url: ROOM_URL,
        audioSource: true,
        videoSource: false,
        dailyConfig: {
            experimentalChromeVideoMuteLightOff: true,
        },
    });

    call.on("joined-meeting", () => {
        document.getElementById('status').innerText = "Baglanti Durumu: Baglandi";
        document.getElementById('fa-check').style.display = "block";
        document.getElementById('fa-exclamation').style.display = "none";

        console.log("[JOINED MEETING]");
    });

    call.on("left-meeting", () => {
        document.getElementById('status').innerText = "Baglanti Durumu: Bagli Degil";
        document.getElementById('fa-check').style.display = "none";
        document.getElementById('fa-exclamation').style.display = "block";
        console.log("[LEFT MEETING]");
    });

    call.on("error", (e) => console.error(e));

    call.on("track-started", playTrack);
    call.on("track-stopped", destroyTrack);
}

async function joinRoom() {
    await call.join();
}

async function leaveRoom() {
    await call.leave();
}

function playTrack(evt) {
    console.log("[TRACK STARTED]", evt.participant && evt.participant.session_id);

    if (!(evt.track && evt.track.kind === "audio")) {
        console.error("!!! playTrack called without an audio track !!!", evt);
        return;
    }

    if (evt.participant.local) {
        return;
    }

    let audioEl = document.createElement("audio");
    document.body.appendChild(audioEl);
    audioEl.srcObject = new MediaStream([evt.track]);
    audioEl.play();
    createVisualizer(audioEl.srcObject); // Create visualizer for audio
}

function destroyTrack(evt) {
    console.log("[TRACK STOPPED]", (evt.participant && evt.participant.session_id) || "[left meeting]");

    let els = Array.from(document.getElementsByTagName("video")).concat(
        Array.from(document.getElementsByTagName("audio"))
    );
    for (let el of els) {
        if (el.srcObject && el.srcObject.getTracks()[0] === evt.track) {
            el.remove();
        }
    }
}

// 3D Visualizer
function createVisualizer(stream) {
    const canvas = document.getElementById('visualizer');
    const ctx = canvas.getContext('2d');
    const audioContext = new (window.AudioContext || window.webkitAudioContext)();
    const analyser = audioContext.createAnalyser();
    const source = audioContext.createMediaStreamSource(stream);
    source.connect(analyser);
    analyser.fftSize = 256;
    const bufferLength = analyser.frequencyBinCount;
    const dataArray = new Uint8Array(bufferLength);
    canvas.width = window.innerWidth;
    canvas.height = 200;

    function draw() {
        requestAnimationFrame(draw);
        analyser.getByteFrequencyData(dataArray);
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.fillStyle = 'rgba(255, 255, 255, 0.5)';
        ctx.strokeStyle = 'white';
        ctx.lineWidth = 2;
        const barWidth = (canvas.width / bufferLength) * 2.5;
        let x = 0;
        for (let i = 0; i < bufferLength; i++) {
            const barHeight = dataArray[i];
            ctx.fillRect(x, canvas.height - barHeight / 2, barWidth, barHeight / 2);
            x += barWidth + 1;
        }
    }

    draw();
}
