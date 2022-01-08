(function () {
    const frame = document.getElementById("frame") as HTMLIFrameElement
    const sender = Math.random() + ""
    frame.src = "https://microsoft.github.io/jacdac-docs/dashboard/#" + sender

    const location = window.location
    const secure = location.protocol === "https:"
    const protocol = secure ? "wss:" : "ws:"
    const hostname = location.hostname
    const port = secure ? 443 : 8081
    const wsurl = `${protocol}//${hostname}:${port}/`

    let ws: WebSocket
    const connectSocket = () => {
        // already connected or connecting
        if (ws) return

        // node.js -> iframe dashboard
        let opened = false
        ws = new WebSocket(wsurl)
        ws.binaryType = "arraybuffer"
        console.debug(`devtools: connecting ${wsurl}...`)
        ws.addEventListener("open", () => {
            console.debug(`devtools: connected ${ws.url}`)
            opened = true
        })
        ws.addEventListener("message", (msg) => {
            const data = new Uint8Array(msg.data)
            const pktMsg = {
                type: "messagepacket",
                channel: "jacdac",
                data,
                sender,
            }
            frame.contentWindow.postMessage(pktMsg, "*")
        })
        ws.addEventListener("close", () => {
            console.debug(`devtools: connection closed`)
            ws = undefined
        })
        ws.addEventListener("error", (e: Event) => {
            if (opened)
                console.debug(`devtools: error`, e)
            ws?.close()
        })
        // iframe dashboard -> node.js
        window.addEventListener("message", msg => {
            const data = msg.data
            if (data && data.type === "messagepacket" && data.channel === "jacdac") {
                if (ws?.readyState === WebSocket.OPEN) {
                    ws.send(data.data)
                }
            }
        })
    }

    // start background connection
    setInterval(connectSocket, 5000);
    connectSocket();
})()