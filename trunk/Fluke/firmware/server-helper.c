#include "fluke.h"
#include "vision.h"
#include "ov7649.h"
#include "uart.h"
#include "scribbler.h"
#include "firmware_upgrade.h"
#include "jpeg.h"
#include "constants.h"


// the image buffer; also used in other RAM intensive operations
// like upgrading the firwmare
unsigned char img[HEIGHT_SIZE*WIDTH_SIZE] __attribute__((aligned(4)));

// buffer that holds a recent scribbler firmware version
extern const unsigned char init_scribbler_prog[INIT_PROG_SIZE];

// used to remember the orientation of the scribbler
//  - is the fluke facing forwards or backwards?
//  - this is stored in location (0,0) in the serial memory
extern unsigned char scribbler_orientation;

// the current state of the battery indicator
unsigned char battery_low = 0;
// a counter to remember when we should check the battery again
int battery_counter = 0;

void update_low_battery_light()
{  
  // check the status of the battery every so often to see if its too low
  if (battery_counter == 1600000)
    {
      if (read_a2d(A2D_CHAN_BAT) < BATTERY_TOO_LOW)
	{		  
	  battery_low = 1;
	}	      
      else
	{
	  if (battery_low)
	    {
	      set_led(0);
	    }
	  battery_low = 0;
	}
      battery_counter = 0;
    }
  
  // toggle the LED if battery is too low
  if (battery_low)
    {
      if ((battery_counter % 80000) == 0)
	{
	  set_led(200);
	}
      else if ((battery_counter % 80000) == 40000)
	{
	  set_led(0);
	}
    }
  
  battery_counter ++;      
}


/*
 * Configure the segmentation routines. 
 *
 * Don't mess with delay. This is a vestigial organ
 *
 * Thresh is a filter that will tell the RLE routine to
 * ignore runs of size less than thresh.
 */	  

void serve_set_rle()
{
  delay =  getchblock() + 21500;
  thresh = getchblock();
  Y_LOW  = getchblock();
  Y_HIGH = getchblock();
  U_LOW  = getchblock();
  U_HIGH = getchblock();
  V_LOW  = getchblock();
  V_HIGH = getchblock();
}


/*
 * Configure the windowing - the height bounds are reversed
 * x ranges from 0-255
 * y ranges from 0-191
 * step sizes can be used to subsample image or to grab only Y,
 * U, or V pixels
 */	  
void serve_set_window()
{
  int i, window;

  window = getchblock();	    
  if (window < 0 || window >= NUM_WINDOWS) window = 0;
  
  i = getchblock();		    
  lower_x[window] = i;	
  
  i = getchblock();
  upper_y[window] = 191-i;
	      
  i = getchblock();
  upper_x[window] = i;
  
  i = getchblock();
  lower_y[window] = 191-i;
  
  i = getchblock();
  xstep[window] = i;
  
  i = getchblock();
  ystep[window] = i;		  	      
}


/*
 * Grab an image and transmit it in raw format
 * 256 x 192 - VYUYVYUY ...
 */
void serve_image()
{
  int i;

  grab_image_vote(IMG_VOTES, img, delay);
  
  for (i = 0; i < WIDTH_SIZE*HEIGHT_SIZE; i++)
    {
      putch(img[i]);
    }
}

void serve_image_window()
{
  int window, i, j;

  window = getchblock(); // which window should I grab?
  if (window < 0 || window >= NUM_WINDOWS) window = 0;
  
  grab_image_vote(IMG_VOTES, img, delay);
  
  for (i = lower_y[window]; i <= upper_y[window]; i += ystep[window])
    {
      for (j = lower_x[window]; j <= upper_x[window]; j += xstep[window])
	{
	  putch(img[i*WIDTH_SIZE + j]);      
	}
    }  
}

void serve_image_window_sum()
{
  int window, i, j;
  int sum, in_counter;
  
  // add up the Y pixels of the window	      
  window = getchblock(); //which window to grab
  if (window < 0 || window >= NUM_WINDOWS) window = 0;
  
  grab_frame(img, delay); 	      
  //grab_image_vote(IMG_VOTES, img, delay);
  
  sum = 0;
  in_counter = 0;
  
  for (i = lower_y[window]; i <= upper_y[window]; i += ystep[window])
    {
      for (j = lower_x[window]; j <= upper_x[window]; j += xstep[window])
	{
	  sum += img[i*WIDTH_SIZE + j];
	  //in_counter ++;
	}
    }
  
  //sum /= in_counter;
  putch(sum >> 16);
  putch(sum >> 8);
  putch(sum);
  //putch(sum);  
}


void serve_rle()
{
  int i;
  get_rle();
  
  // send the size of the RLE buffer as 2 byte value 
  putch(rle_counter >> 8);
  putch(rle_counter);
  
  // send the buffer
  for (i = 0; i < rle_counter; i++)
    {
      putch(rle[i]);
    }	    	            
}


void serve_blob()
{
  int x, y;
  int pix_counter = 0;
  get_blob(&pix_counter, &x, &y);
  
  putch(pix_counter >> 8);
  putch(pix_counter);
  putch(x);
  putch(y);
}

void serve_blob_window()
{
  int x, y;
  int pix_counter = 0;
  get_blob_window(&pix_counter, &x, &y);
  
  putch(pix_counter >> 8);
  putch(pix_counter);
  putch(x);
  putch(y);
}

void serve_jpeg_gray_header()
{
  int i;
  putch(gray_header_size & 0xff);
  putch((gray_header_size >> 8) & 0xff);
  for (i = 0; i < gray_header_size; i++)
    {
      putch(gray_header[i]);
    }
}


void serve_jpeg_color_header()
{
  int i;
  putch(color_header_size & 0xff);
  putch((color_header_size >> 8) & 0xff);
  for (i = 0; i < color_header_size; i++)
    {
      putch(color_header[i]);
    }
}

void serve_jpeg_gray_scan()
{
  int i;
  i = getchblock();
  uint32_t bm0 = TIMER0_TC;
  if (i)
    {
      grab_image_vote(i, img, delay);
    }
  else
    {
      grab_frame(img, delay);        
    }
  uint32_t bm1 = TIMER0_TC;
  jpeg_compress(0);
  uint32_t bm2 = TIMER0_TC;
  write_word(bm0);
  write_word(bm1);
  write_word(bm2);
}

void serve_jpeg_color_scan()
{
  int i;
  i = getchblock();
  uint32_t bm0 = TIMER0_TC;
  if (i)
    {
      grab_image_vote(i, img, delay);
    }
  else
    {
      grab_frame(img, delay);        
    }
  uint32_t bm1 = TIMER0_TC;
  jpeg_compress(1);
  uint32_t bm2 = TIMER0_TC;
  write_word(bm0);
  write_word(bm1);
  write_word(bm2);
}

/*
 * Return the battery voltage a 2 byte value
 * to convert to volts: value / 20.9813
 */
void serve_battery()
{
  int value;
  value = read_a2d(A2D_CHAN_BAT);
  putch2b(value);
}

/*
 * Set backward (configurable) led
 */
void serve_back_led()
{
  int value;
  value = getchblock();
  set_led(value);
}

/*
 * Set IR transmit power
 */
void serve_ir_power()
{
  int value;
  value = getchblock();
  set_ir(value);
}

/*
 * Read a serial memory location
 */
void serve_get_serial_mem()
{
  int page, offset;
  int value;
  
  value = 0;
  
  page = getchblock2b();  
  offset = getchblock2b();
  
  read_mem(page, offset, (uint8_t*)&value, 1);
  
  putch(value);
}

/*
 * Write a serial memory location (doesn't validate address)
 */
void serve_set_serial_mem()
{
  int page, offset, value;
  
  page = getchblock2b();
  offset = getchblock2b();
  
  value = getchblock();
  
  write_mem(page, offset, (uint8_t*)&value, 1);  
}


/*
 * Erase a page of the serial memory
 */
void serve_erase_serial_mem()
{
  int page;
  page = getchblock();
  page = (page << 8);
  page = page | getchblock();	      
  erase_mem(page);
}

/*
 * Process an IR message - uses one or the 3 emitters and the receiver
 * to detect an obstacle.
 * 
 * Pulses IR periodically and keeps a counter of reflected IR.  If it
 * receives IR when it itself isn't sending it decrements the counter
 * to trya nd remove false positives.
 *
 * This can definitely be improved.
 */
void serve_ir(uint8_t emitters)
{
  int pulses;
  pulses = check_ir_bounce(emitters);
  putch2b(pulses);		  
}

/*
 * Begin filling the scribbler program buffer. (Also the image buffer)
 */
void serve_set_scrib_program()
{
  int addr;

  // get the address
  addr = getchblock();
  addr = (addr << 8);
  addr = addr | getchblock();
  
  // store the byte
  if (addr < WIDTH_SIZE*HEIGHT_SIZE)
    {
      img[addr] = getchblock();
    }
}

/*
 * Read the scribbler program buffer. (Also the image buffer)
 */
void serve_get_scrib_program()
{
  int addr;
  // get the address
  addr = getchblock();
  // return the byte
  putch(img[addr]);
}


/*
 * Begin programming the scribbler. Assumes you have filled the buffer
 * with a valid program or are using the one stored in this firmware
 * (indicated by and program size of 0).
 *
 * A magic code is expected to enter program mode. Used to reduce
 * unintended reprogramming.
 */

void serve_start_scrib_program()
{
  int code, addr;

  code = getchblock2b();		  
  addr = getchblock2b();	      		  
  
  // magic code to enable scribbler programming
  if (code == 0x0123)
    {	
      set_led(0);
      led_off();
      if (addr == 0)
	{
	  program_scribbler((unsigned char*)init_scribbler_prog, 
			    INIT_PROG_SIZE);
	}	      
      else
	{
	  led_on();
	  program_scribbler(img, addr);
	  led_off();
	}
      msleep(250);
      reset_scribbler();
    }  
}

/*
 * reset the scribbler
 */
void serve_reset_scribbler()
{
  reset_scribbler();
}

/*
 * Used to reconfigure UART0
 *  - Intended for use with other robots 
 */
void serve_set_uart0()
{
  int key;
  int baud;
  int format;
  int fifo;
  // get the magic key
  key = getchblock();
  if (key == 0x01)
    {
      baud = getchblock(); //getchblock2b();
      format = getchblock(); //getchblock2b();
      fifo = getchblock(); //getchblock2b();
      uart0Init(baud, format, fifo); // setup UART 0
    }  
}


/*
 * Pass a byte to the serial port (UART0)
 */
void serve_send_byte()
{
  int ch;
  // get the address
  ch = getchblock();
  // return the byte
  uart0Putch(ch);
}

/*
 * Enter a passthrough mode where no commands will interpretted by the
 * fluke. Need a magic code of 0x01 to enter after the command code
 */
void serve_set_passthrough()
{
  int ch, code, i;
  
  code = getchblock();
  if (code == 0x01)
    {		  
      for (i = 0; i < 200; i++)
	{
	  ch = uart1Getch();
	}
      
      while (1)
	{		
	  // receive from bluetooth send to DB9
	  ch = getch();
	  if (ch != -1)
	    {
	      //set_led(200);
	      uart0Putch(ch);
	    }
	  
	  ch = uart0Getch();
	  if (ch != -1)
	    {
	      //led_on();
	      putch(ch);
	    }
	}
    }  
}


/*
 * Set the orientation of the scribbler
 * 0: Scribbler forward
 * 1: Fluke forward
 */
void serve_set_forwardness()
{
  scribbler_orientation = getchblock();
  if (scribbler_orientation == 0)
    {
      scribbler_orientation = 0xDF;
      write_mem(0, 0, &scribbler_orientation, 1);  
      scribbler_orientation = 0;
    }
  else
    {
      scribbler_orientation = 1;
      write_mem(0, 0, &scribbler_orientation, 1);  		  
    }
}

/*
 * Turn on white balance
 */
void serve_set_wb()
{
  write_camera(OV_COMA, 0x14 | (1<<2));

}

/*
 * Turn off white balance
 */
void serve_unset_wb()
{
  write_camera(OV_COMA, 0x14 & ~(1<<2));
}


/*
 * Read a camera configuration register
 */
void serve_read_camera()
{
  int addr, value;
  addr = getchblock();
  read_camera((uint8_t)addr, (uint8_t*)&value);
  putch(value);
}

/*
 * Write a camera configuration register
 */
void serve_write_camera()
{
  int addr, value;
  addr = getchblock();
  value = getchblock();
  write_camera(addr, value);
}

	      
/*
 * Dump the serial memory in preparation for a firmware upgrade
 */
void serve_save_eeprom()
{
  uf_saveEEPROMdump();		  
}

/*
 * Reload the serial memory (after a firmware upgrade)
 */

void serve_restore_eeprom()
{
  uf_restoreEEPROMdump();
}

/*
 * Initiate Firmware Upgrade - uses a magic code to prevent unintended
 * programming
 */
void serve_update_firmware()
{
  int code;
  // get a special code
  code = getchblock2b();
  
  // magic code to enable scribbler programming
  if (code == 0x0123)
    {		  		  
      set_led(0);
      led_on();
		      uf_storeinEEPROM();
		      ram_uf_loadFlash();
    }
}

/*
 * Reset Fluke using watchdog
 */
void serve_fluke_reset()
{
  //reset
  WDTC = 0x00FFF; 
  WDMOD = 0x03; 
  WDFEED= 0xAA;
  WDFEED= 0x55;
}