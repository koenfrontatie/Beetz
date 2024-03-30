<CsoundSynthesizer>
<CsOptions>
-odac ; activate real-time audio output
</CsOptions>
<CsInstruments>
sr 	= 	44100
kr = 100
ksmps 	= 	441
nchnls 	= 	1	

  instr	1 ; play audio from disk
kSpeed  init     1           ; playback speed
iSkip   init     0           ; inskip into file (in seconds)
iLoop   init     0           ; looping switch (0=off 1=on)
; read audio from disk using diskin2 opcode
a1      diskin2  "1kick.wav", kSpeed, iSkip, iLoop
        out      a1          ; send audio to outputs
  endin
  
  instr	2 ; play audio from disk
kSpeed  init     1           ; playback speed
iSkip   init     0           ; inskip into file (in seconds)
iLoop   init     0           ; looping switch (0=off 1=on)
; read audio from disk using diskin2 opcode
a1      diskin2  "2hat.wav", kSpeed, iSkip, iLoop
        out      a1          ; send audio to outputs
  endin
  
  instr	3 ; play audio from disk
kSpeed  init     1           ; playback speed
iSkip   init     0           ; inskip into file (in seconds)
iLoop   init     0           ; looping switch (0=off 1=on)
; read audio from disk using diskin2 opcode
a1      diskin2  "3clap.wav", kSpeed, iSkip, iLoop
        out      a1          ; send audio to outputs
  endin
  
  instr	4 ; play audio from disk
kSpeed  init     1           ; playback speed
iSkip   init     0           ; inskip into file (in seconds)
iLoop   init     0           ; looping switch (0=off 1=on)
; read audio from disk using diskin2 opcode
a1      diskin2  "4cow.wav", kSpeed, iSkip, iLoop
        out      a1          ; send audio to outputs
  endin
  
  instr	5 ; play audio from disk
kSpeed  init     1           ; playback speed
iSkip   init     0           ; inskip into file (in seconds)
iLoop   init     0           ; looping switch (0=off 1=on)
; read audio from disk using diskin2 opcode
a1      diskin2  "5snare.wav", kSpeed, iSkip, iLoop
        out      a1          ; send audio to outputs
		;aL, aR  freeverb a1, a1, 0.9, 0.7, sr, 0
		;outs a1 + aL, a1 + aR
  endin
  
    instr	6 ; play audio from disk
kSpeed  init     1           ; playback speed
iSkip   init     0           ; inskip into file (in seconds)
iLoop   init     0           ; looping switch (0=off 1=on)
; read audio from disk using diskin2 opcode
a1      diskin2  "6kick808.wav", kSpeed, iSkip, iLoop
        outs      a1         ; send audio to outputs
  endin
  
    instr	7 ; play audio from disk
kSpeed  init     1           ; playback speed
iSkip   init     0           ; inskip into file (in seconds)
iLoop   init     0           ; looping switch (0=off 1=on)
; read audio from disk using diskin2 opcode
a1      diskin2  "7snare2.wav", kSpeed, iSkip, iLoop
        outs      a1          ; send audio to outputs
  endin
</CsInstruments>

<CsScore>


f0 z 

</CsScore>
</CsoundSynthesizer>