package com.agenda.util;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;

public class DBUtil {
    private static final String DB_URL = "jdbc:oracle:thin:@localhost:1521:xe";
    private static final String USER   = "SYSTEM";
    private static final String PASS   = "43026172LNRm@";

    public static Connection getConnection() throws SQLException {
        return DriverManager.getConnection(DB_URL, USER, PASS);
    }
}
