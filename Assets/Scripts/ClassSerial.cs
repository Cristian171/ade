using System;
using System.IO.Ports;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SerialCommunication : MonoBehaviour
{
    private SerialPort serialPort;

    private string portName = "COM4"; // Asegúrate de usar el puerto correcto en tu sistema
    private int baudRate = 115200;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _text2;
    [SerializeField] private TextMeshProUGUI _timerText; // Texto para mostrar el tiempo

    // Referencias de los botones
    [SerializeField] private Button encenderLedButton;
    [SerializeField] private Button apagarLedButton;
    [SerializeField] private Button subirPresionButton;
    [SerializeField] private Button bajarPresionButton;
    [SerializeField] private Button subirNivelButton;
    [SerializeField] private Button bajarNivelButton;
    [SerializeField] private Button iniciarButton; // Botón para iniciar el conteo

    private float countdownTime = 60f; // Tiempo de cuenta regresiva en segundos
    private bool isCountingDown = false;

    void Start()
    {
        // Inicializar el puerto serial
        serialPort = new SerialPort(portName, baudRate);
        serialPort.DtrEnable = true;
        serialPort.NewLine = "\n";

        try
        {
            serialPort.Open();
            Debug.Log("Conexión establecida con Raspberry Pi Pico");
        }
        catch (Exception e)
        {
            Debug.LogError("Error al abrir el puerto serial: " + e.Message);
            return; // Salir si no se puede abrir el puerto
        }

        // Verificar las referencias de los objetos TextMeshProUGUI
        if (_text == null)
        {
            Debug.LogError("_text no está asignado.");
        }
        if (_text2 == null)
        {
            Debug.LogError("_text2 no está asignado.");
        }
        if (_timerText == null)
        {
            Debug.LogError("_timerText no está asignado.");
        }

        // Asignar las funciones de los botones
        encenderLedButton.onClick.AddListener(EncenderLed);
        apagarLedButton.onClick.AddListener(ApagarLed);
        subirPresionButton.onClick.AddListener(SubirPresion);
        bajarPresionButton.onClick.AddListener(BajarPresion);
        subirNivelButton.onClick.AddListener(SubirNivel);
        bajarNivelButton.onClick.AddListener(BajarNivel);
        iniciarButton.onClick.AddListener(IniciarConteo); // Asignar el evento al botón de iniciar conteo

        Debug.Log("Se asignaron los eventos a los botones.");
    }

    void Update()
    {
        // Verificar si el puerto serial está abierto antes de usarlo
        if (serialPort == null)
        {
            Debug.LogError("El puerto serial no está inicializado.");
            return;
        }

        if (serialPort.IsOpen)
        {
            // Leer datos si están disponibles
            if (serialPort.BytesToRead > 0)
            {
                string receivedData = serialPort.ReadLine();
                Debug.Log("Datos recibidos: " + receivedData);

                // Procesar los datos recibidos
                ProcessReceivedData(receivedData);
            }
        }
        else
        {
            Debug.LogWarning("Puerto serial cerrado.");
        }

        // Si la cuenta regresiva está activa, disminuye el tiempo
        if (isCountingDown)
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime <= 0)
            {
                countdownTime = 0;
                isCountingDown = false;
                Debug.Log("El tiempo se ha agotado.");
            }

            // Actualizar el texto del temporizador
            _timerText.text = "Tiempo restante: " + Mathf.Ceil(countdownTime).ToString() + " s";
        }
    }

    // Función para iniciar la cuenta regresiva
    private void IniciarConteo()
    {
        Debug.Log("IniciarConteo() llamado"); // Depuración

        if (!isCountingDown)
        {
            Debug.Log("Iniciando cuenta regresiva."); // Depuración
            countdownTime = 60f; // Reiniciar el temporizador a 60 segundos
            isCountingDown = true; // Iniciar la cuenta regresiva

            SendCommand("Iniciar"); // Enviar comando a Raspberry Pi Pico
        }
    }

    // Función para enviar comandos al Raspberry Pi Pico
    private void SendCommand(string command)
    {
        if (serialPort.IsOpen)
        {
            serialPort.WriteLine(command);
            Debug.Log("Comando enviado: " + command);
        }
        else
        {
            Debug.LogWarning("Puerto serial no abierto. No se puede enviar el comando.");
        }
    }

    // Función para encender el LED
    private void EncenderLed()
    {
        Debug.Log("Enviando comando para encender el LED...");
        SendCommand("EncenderLed");
    }

    // Función para apagar el LED
    private void ApagarLed()
    {
        Debug.Log("Enviando comando para apagar el LED...");
        SendCommand("ApagarLed");
    }

    // Función para subir la presión
    private void SubirPresion()
    {
        SendCommand("SubirPresion");
    }

    // Función para bajar la presión
    private void BajarPresion()
    {
        SendCommand("BajarPresion");
    }

    // Función para subir el nivel
    private void SubirNivel()
    {
        SendCommand("SubirNivel");
    }

    // Función para bajar el nivel
    private void BajarNivel()
    {
        SendCommand("BajarNivel");
    }

    // Procesar los datos recibidos y mostrarlos en el TextMeshProUGUI
    private void ProcessReceivedData(string data)
    {
        // Suponiendo que el formato de los datos recibidos es una cadena
        if (_text != null)
        {
            _text.text = "Datos recibidos: " + data;
        }

        // Si tienes más datos o deseas procesarlos de una manera específica, agrégalo aquí
        if (_text2 != null)
        {
            _text2.text = "Datos adicionales: " + data; // O realiza alguna otra operación con los datos
        }
    }

    // Asegúrate de cerrar el puerto serial cuando se cierre la aplicación o el objeto
    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("Puerto serial cerrado.");
        }
    }
}
