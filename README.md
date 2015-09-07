# IBIStools

Programmme zum Senden von Nachrichten an IBIS kompatible (VDV 300 Wagenbus) Anzeigen mit serieller Schnittstelle.

* IBIScmdline Kommandozeilentool. Aufruf 'IBIScmdline COM1 "l123"
* IBISui grafische Oberfläche zur Ansteuerung vin IBIS Linien- bzw. Haltestellen-Anzeigen.

## Features

* derzeit unterstützte Befehle, Liniennummer (DS001),  Sonderzeichen (DS001), 2 Zeilen Zielanzeige (DS003), 4 Zeilen Zielanzeige (DS003, DS003aMAS), Haltestellenanzeige (DS003C, DS009) 
* Emulation einer 1200 Baud, 7 databits, even parity, 2 stopbits (1200,7,e,2) Schnittstelle (Standard IBIS) über eine Bluetooth Übertragung mit 1200 Baud 8 datenbits, no parity und 1 stopbit (1200,8,n,1)

## Anforderungen

* Flip-Dot/LED Anzeige mit IBIS Wagenbus-Schnittstelle
* Seriell zu IBIS Schnittstellen Konverter, siehe [IBIS-Wandler](https://github.com/robotfreak/IBIScmdline/tree/master/IBIS-Wandler)
* optional Bluetoth Modul mit Einstellmöglichkeit für 1200 Baud (z.B. HC-06, BTM222, HM-10)
* .NET 4.0, VisualStudio 2015

Eine übersetzte Version der IBISui kann hier heruntergeladen werden:
[IBISui](http://www.robotfreak.de/IBISui/publish.htm)
