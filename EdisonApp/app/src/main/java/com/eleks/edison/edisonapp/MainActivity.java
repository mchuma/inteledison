package com.eleks.edison.edisonapp;

import android.app.Notification;
import android.app.NotificationManager;
import android.content.Context;
import android.content.Intent;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.StrictMode;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.Toast;
import android.widget.ToggleButton;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.net.HttpURLConnection;
import java.net.Socket;
import java.net.URL;
import java.net.UnknownHostException;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().permitAll().build();
        StrictMode.setThreadPolicy(policy);

        final Button button = (Button) findViewById(R.id.button);
        Boolean conn = checkInternetConnection();
        button.setEnabled(conn);

        button.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                try {
                    request("blink");
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        });

        final Button button2 = (Button) findViewById(R.id.button2);
        button2.setEnabled(conn);

        button2.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                try {
                    request("shutdown");
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        });

        ToggleButton toggle = (ToggleButton) findViewById(R.id.toggleButton);
        toggle.setEnabled(conn);
        toggle.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
                if (isChecked) {
                    try {
                        request("startlogging");
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                } else {
                    try {
                        request("stoplogging");
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                }
            }
        });
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        switch (item.getItemId()) {
            case R.id.action_exit:
                android.os.Process.killProcess(android.os.Process.myPid());
                System.exit(1);
            case R.id.action_settings:
                Intent myIntent = new Intent(MainActivity.this, SettingsActivity.class);
                MainActivity.this.startActivity(myIntent);
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    private void request(String cmd) throws IOException {
        try {
            Socket socket = new Socket("172.19.17.17", 5005);
            String str = cmd;
            PrintWriter out = new PrintWriter(new BufferedWriter(
                    new OutputStreamWriter(socket.getOutputStream())), true);
            out.println(str);
        } catch (UnknownHostException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    //check Internet connection.
    private Boolean checkInternetConnection() {
        ConnectivityManager connManager = (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);
        NetworkInfo mWifi = connManager.getNetworkInfo(ConnectivityManager.TYPE_WIFI);

        if (mWifi.isConnected()) {
            WifiManager wifiManager = (WifiManager) getSystemService(Context.WIFI_SERVICE);
            WifiInfo info = wifiManager.getConnectionInfo();
            info.getBSSID();
            showMessage("Connected to " + info.getSSID());
            return true;
        } else {
            showMessage("No WIFI connection");
            return false;
        }
    }

    private void showMessage(String text) {
        Context context = getApplicationContext();
        int duration = Toast.LENGTH_SHORT;
        Toast toast = Toast.makeText(context, text, duration);
        toast.show();
    }
}
